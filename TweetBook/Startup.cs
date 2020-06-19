using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Linq;
using TweetBook.Contract.HealthChecks;
using TweetBook.Installers;
using TweetBook.Options;

namespace TweetBook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallServicesInAssemlbies(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => {
                option.RouteTemplate = swaggerOptions.JsonRoute;
            });
            app.UseSwaggerUI(option => {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint,swaggerOptions.Description);
            
            });


            app.UseHealthChecks("/health",new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { 
            ResponseWriter=async(context, report) => {
                context.Response.ContentType = "application/json";
                var response = new HealthCheckResponse
                {
                    Status=report.Status.ToString(),
                    Checks=report.Entries.Select(x=>new HealthCheck { 
                     Status=x.Value.Status.ToString(),
                     Description=x.Value.Description,
                     Component= x.Key
                    }),
                    Duartion=report.TotalDuration
                };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
            });


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
