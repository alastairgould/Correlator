using System;
using System.Net.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Correlator.IntergrationTests
{
    public class StartupTest
    {
        public StartupTest(IHostingEnvironment env, IConfiguration config)
        {
            HostingEnvironment = env;
            Configuration = config;
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddControllersAsServices();

            services.AddScoped<RequestCorrelationId>();
            services.AddHttpContextAccessor();
          
            services.AddTransient(provider =>
            {
                return new CorrelatorIncomingMiddleware("X-Correlation-ID", () => Guid.NewGuid().ToString(), provider.GetRequiredService<RequestCorrelationId>() );
            });

            services.AddTransient(provider => new CorrelatorOutgoingMiddleware("X-Correlation-ID",
                provider.GetRequiredService<IHttpContextAccessor>()));

            services.AddTransient<TestController>();

            services.AddHttpClient("secondcall", c => { c.BaseAddress = new Uri("http://localhost"); })
                .AddHttpMessageHandler<CorrelatorOutgoingMiddleware>();

            // Configuration is available during startup. Examples:
            // Configuration["key"]
            // Configuration["subsection:suboption1"]
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<CorrelatorIncomingMiddleware>();
            app.UseMvcWithDefaultRoute();
        }
    }
}