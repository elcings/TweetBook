using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TweetBook;
using TweetBook.Data;
using TweetBook.Contract.V1;
using TweetBook.Contract.V1.Models;
using Newtonsoft.Json;

namespace TweetBokk.Integration
{
    public class IntegrationTest
    {
      protected  readonly HttpClient _httpClient;
        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder=> {

                    builder.ConfigureServices(services =>
                    {

                        var descriptor = services.SingleOrDefault(
                          d => d.ServiceType ==
                             typeof(DbContextOptions<DataContext>));

                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        var serviceProvider = new ServiceCollection()
                          .AddEntityFrameworkInMemoryDatabase()
                          .BuildServiceProvider();

                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDB");
                            options.UseInternalServiceProvider(serviceProvider);
                        });

                        var sp = services.BuildServiceProvider();

                        using (var scope = sp.CreateScope())
                        {
                            using (var appContext = scope.ServiceProvider.GetRequiredService<DataContext>())
                            {
                                try
                                {
                                    appContext.Database.EnsureCreated();
                                }
                                catch (Exception ex)
                                {
                                    //Log errors or do anything you think it's needed
                                    throw;
                                }
                            }
                        }
                    });
                }
            );
            _httpClient = appFactory.CreateClient();
        }

        protected async Task Auth()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",await getJwtToken());
        }

        private async Task<string> getJwtToken()
        {
            var payload = JsonConvert.SerializeObject(new RegisterModel
            {
                FirstName="test",
                LastName="test",
                Email="test@g.com",
                Password="123"
            });
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiRoute.Users.Register, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<AuthenticateResponse>(res);
                return model.Token;
            }
            return "";
        }

        protected async Task<Guid> CreatePost(CreatePostModel model)
        {
            var payload = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiRoute.Posts.Create, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                var mod = JsonConvert.DeserializeObject<Guid>(res);
                return mod;
            }
            return Guid.Empty;
        }
    }
}
