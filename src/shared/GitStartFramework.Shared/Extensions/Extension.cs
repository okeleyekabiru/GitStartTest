using GitStartFramework.Shared.Persistence;
using GitStartFramework.Shared.Persistence.Repository;
using GitStartFramework.Shared.Persistence.Repository.interfaces;
using GitStartFramework.Shared.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;
using System.Runtime.CompilerServices;

namespace GitStartFramework.Shared.Extensions
{
    public static class Extension
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }

        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var rabbitMqUsername = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "guest";
            var rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "guest";

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

        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
            var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");

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

        public static void AddLogging(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Environment.GetEnvironmentVariable("Logging_Url")!))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{Environment.GetEnvironmentVariable("Application_Name")}-logs-{DateTime.UtcNow:yyyy.MM.dd}",
                    MinimumLogEventLevel = LogEventLevel.Information
                })
                .CreateLogger();
        }
    }
}