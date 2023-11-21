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
using Neuroglia.Mediation;
using Neuroglia.Mediation.Services;
using Neuroglia.Serialization;
using System.Net.Mime;

namespace Neuroglia.UnitTests.Cases.Eventing.CloudEvents;

public class CloudEventIngestorTests
    : IAsyncLifetime
{

    const string Type = "io.neuroglia.framework.unit-tests.fake";

    public CloudEventIngestorTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSerialization();
        services.AddJsonSerializer();
        services.AddMediator();
        services.AddCloudEventBus();
        services.AddCloudEventIngestor(options => options.IngestEventsOfType<Contact>(Type));
        services.AddTransient<INotificationHandler<Contact>>(provider => new DelegateNotificationHandler<Contact>(provider, (provider, contact, token) =>
        {
            this.LastIngestedContact = contact;
            return Task.CompletedTask;
        }));
        this.ServiceProvider = services.BuildServiceProvider();
        this.CloudEventBus = this.ServiceProvider.GetRequiredService<ICloudEventBus>();
        this.CloudEventIngestor = this.ServiceProvider.GetRequiredService<ICloudEventIngestor>();
    }

    protected ServiceProvider ServiceProvider { get; }

    protected ICloudEventBus CloudEventBus { get; }

    protected ICloudEventIngestor CloudEventIngestor { get; }

    protected Contact? LastIngestedContact { get; private set; }

    public virtual async Task InitializeAsync()
    {
        foreach (var hostedService in this.ServiceProvider.GetServices<IHostedService>())
        {
            await hostedService.StartAsync(default);
        }
    }

    public virtual async Task DisposeAsync() => await this.ServiceProvider.DisposeAsync().ConfigureAwait(false);

    [Fact]
    public async Task Ingest_CloudEvent_Programatically_Should_Work()
    {
        //arrange
        var id = Guid.NewGuid().ToString("N");
        var source = new Uri("https://unit-tests.framework.neuroglia.io");
        var data = new Contact("fake-name", "fake-phone-number");
        var dataContentType = MediaTypeNames.Application.Json;
        var e = new CloudEvent()
        {
            Id = id,
            Source = source,
            Type = Type,
            DataContentType = dataContentType,
            Data = data
        };

        //act
        await this.CloudEventIngestor.IngestAsync(e);

        //assert
        this.LastIngestedContact.Should().NotBeNull();
        this.LastIngestedContact.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void Ingest_CloudEvent_UsingBus_Should_Work()
    {
        //arrange
        var id = Guid.NewGuid().ToString("N");
        var source = new Uri("https://unit-tests.framework.neuroglia.io");
        var data = new Contact("fake-name", "fake-phone-number");
        var dataContentType = MediaTypeNames.Application.Json;
        var e = new CloudEvent()
        {
            Id = id,
            Source = source,
            Type = Type,
            DataContentType = dataContentType,
            Data = data
        };

        //act
        this.CloudEventBus.InputStream.OnNext(e);

        //assert
        this.LastIngestedContact.Should().NotBeNull();
        this.LastIngestedContact.Should().BeEquivalentTo(data);
    }

}
