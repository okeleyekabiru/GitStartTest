using GitStartFramework.Shared.Configuration;
using GitStartFramework.Shared.Persistence;
using GitStartFramework.Shared.Persistence.Repository;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using GitStartFramework.Shared.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace GitStartFramework.Shared.Extensions
{
    public static class Extension
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, s => s.MigrationsAssembly(assembly.FullName)));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }

        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqHost = configuration["RABBITMQ_HOST"];
            var rabbitMqUsername = configuration["RABBITMQ_DEFAULT_USER"];
            var rabbitMqPassword = configuration["RABBITMQ_DEFAULT_PASS"];

            var factory = new ConnectionFactory
            {
                HostName = rabbitMqHost,
                UserName = rabbitMqUsername,
                Password = rabbitMqPassword,
                DispatchConsumersAsync = true
            };

            services.AddSingleton<IConnectionFactory>(factory);
            services.AddSingleton(factory.CreateConnection());

            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisHost = configuration["REDIS_HOST"];
            var redisPassword = configuration["REDIS_PASSWORD"];

            var connectionString = $"{redisHost},password={redisPassword}";
            var redis = ConnectionMultiplexer.Connect(connectionString);
            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });

            services.AddSingleton<IRedisService, RedisService>();
            return services;
        }

        public static void MigrateDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }
        }

        public static void AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["Logging_Url"]!))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{configuration["Application_Name"]}-logs-{DateTime.UtcNow:yyyy.MM.dd}",
                    MinimumLogEventLevel = LogEventLevel.Information
                })
                .CreateLogger();
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]))
                };
            });
            return services;
        }
    }
}