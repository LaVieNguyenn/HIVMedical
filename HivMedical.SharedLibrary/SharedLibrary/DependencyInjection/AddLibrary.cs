using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SharedLibrary.Jwt;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.ServiceClients;
using SharedLibrary.Messaging;
using RabbitMQ.Client;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;

namespace SharedLibrary.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLibrary(this IServiceCollection services, IConfiguration configuration)
        {
            // Enforce secure HTTPS connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings = configuration.GetSection("Jwt").Get<JwtOptions>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });
            services.AddAuthorization();
            
            // Register service clients with secure HTTPS
            services.AddHttpClient<AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ServiceUrls:AuthService"] ?? "https://localhost:5001");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            });
            
            return services;
        }
        
        public static IServiceCollection AddServiceClients(this IServiceCollection services, IConfiguration configuration)
        {
            // Register service clients with secure HTTPS
            services.AddHttpClient<AuthServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ServiceUrls:AuthService"] ?? "https://localhost:5001");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            });
            
            return services;
        }
        
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure RabbitMQ
            services.AddSingleton<IConnectionFactory>(sp => 
                new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                    UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = configuration["RabbitMQ:Password"] ?? "guest",
                    Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672")
                });
            
            // Register the event bus
            services.AddSingleton<IEventBus, RabbitMQEventBus>();
            
            return services;
        }
    }
}
