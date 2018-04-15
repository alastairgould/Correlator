using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Correlator.IntergrationTests
{
    public class WebApplicationFactoryWebHostBuilder<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder().UseStartup<StartupTest>()
                .UseUrls("http://localhost");

        }
    }
}