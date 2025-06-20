using Authentication.Application.Interfaces;
using Authentication.Application.Services;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Repositories;
using Authentication.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Jwt;

namespace Authentication.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
            // Repository registrations
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Unit of Work registration
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // Register services
            services.AddScoped<AuthService>();
            services.AddScoped<UserManagementService>();
            services.AddScoped<IJwtService, JwtService>();
            
            return services;
        }
    }
}
