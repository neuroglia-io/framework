// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.Eventing.CloudEvents;
using Neuroglia.Eventing.CloudEvents.Infrastructure;
using Neuroglia.Eventing.CloudEvents.Infrastructure.Services;
using Neuroglia.Mapping;
using Neuroglia.Serialization;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Mime;

namespace Neuroglia.UnitTests.Cases.Eventing.CloudEvents;

public class CloudEventPublisherTests
    : IAsyncLifetime
{

    public CloudEventPublisherTests()
    {
        var sinkUri = new Uri("https://unit-tests.framework.neuroglia.io/cloud-events/pub");
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Post, sinkUri.OriginalString)
            .Respond(HttpStatusCode.Accepted, message =>
            {
                this.PublishedEventsCount++;
                return new StringContent(string.Empty);
            });
        var client = new HttpClient(mockHttp);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddJsonSerializer();
        services.AddCloudEventBus();
        services.AddCloudEventPublisher(options => options.Sink = sinkUri);
        services.AddSingleton(client);
        this.ServiceProvider = services.BuildServiceProvider();
        this.CloudEventBus = this.ServiceProvider.GetRequiredService<ICloudEventBus>();
        this.CloudEventPublisher = this.ServiceProvider.GetRequiredService<ICloudEventPublisher>();
    }

    protected ServiceProvider ServiceProvider { get; }

    protected ICloudEventBus CloudEventBus { get; }

    protected ICloudEventPublisher CloudEventPublisher { get; }

    protected int PublishedEventsCount { get; private set; }

    public virtual async Task InitializeAsync()
    {
        foreach(var hostedService in this.ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(default);
        }
    }

    public virtual async Task DisposeAsync() => await this.ServiceProvider.DisposeAsync().ConfigureAwait(false);

    [Fact]
    public async Task Publish_CloudEvent_Programatically_Should_Work()
    {
        //arrange
        var id = Guid.NewGuid().ToString("N");
        var source = new Uri("https://unit-tests.framework.neuroglia.io");
        var type = "io.neuroglia.framework.unit-tests.fake";
        var data = new Contact("fake-name", "fake-phone-number");
        var dataContentType = MediaTypeNames.Application.Json;
        var e = new CloudEvent()
        {
            Id = id,
            Source = source,
            Type = type,
            DataContentType = dataContentType,
            Data = data
        };

        //act
        await this.CloudEventPublisher.PublishAsync(e);

        //assert
        this.PublishedEventsCount.Should().Be(1);
    }

    [Fact]
    public void Publish_CloudEvent_UsingBus_Should_Work()
    {
        //arrange
        var id = Guid.NewGuid().ToString("N");
        var source = new Uri("https://unit-tests.framework.neuroglia.io");
        var type = "io.neuroglia.framework.unit-tests.fake";
        var data = new Contact("fake-name", "fake-phone-number");
        var dataContentType = MediaTypeNames.Application.Json;
        var e = new CloudEvent()
        {
            Id = id,
            Source = source,
            Type = type,
            DataContentType = dataContentType,
            Data = data
        };

        //act
        this.CloudEventBus.OutputStream.OnNext(e);

        //assert
        this.PublishedEventsCount.Should().Be(1);
    }

}
