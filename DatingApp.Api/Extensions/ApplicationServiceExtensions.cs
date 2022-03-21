using DatingApp.Api.Data;
using DatingApp.Api.Interfaces;
using DatingApp.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Add services to the container.
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
