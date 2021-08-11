using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Templating;
using Neuroglia.Templating.Services;
using Neuroglia.UnitTests.Cases.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Neuroglia.UnitTests.Cases.Templating
{

    public class RazorLightTemplateRendererTests
    {

        public RazorLightTemplateRendererTests()
        {
            ServiceCollection services = new();
            services.AddLogging();
            services.AddRazorLightTemplateRenderer(builder =>
            {
                builder.UseEmbeddedResourcesProject(typeof(RazorLightTemplateRendererTests))
                    .SetOperatingAssembly(typeof(RazorLightTemplateRendererTests).Assembly)
                    .UseMemoryCachingProvider();
            }, ServiceLifetime.Singleton);
            this.TemplateRenderer = services.BuildServiceProvider().GetRequiredService<ITemplateRenderer>();
        }

        ITemplateRenderer TemplateRenderer { get; }

        [Fact]
        public async Task RenderTemplate()
        {
            //arrange
            var template = @"
    @model Neuroglia.UnitTests.Cases.Data.TestModel;
    <select>
        @foreach(string option in this.Model.Options)
        {
            <option value=""@option"">@option</option>
        }
    </select>
";

            //act
            var rendered = await this.TemplateRenderer.RenderTemplateAsync(template, new TestModel());

            //assert
            rendered.Should().NotBeNullOrWhiteSpace();
        }

    }

}
