using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Data;
using TweetBook.Services;

namespace TweetBook.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(option => {
                option.UseSqlServer(configuration.GetConnectionString("PostConnectionString"));
            });
            RegisterServices(services);
            RegisterMapper(services);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserService, UserService>();
        }
        private void RegisterMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

        }
    }
}
