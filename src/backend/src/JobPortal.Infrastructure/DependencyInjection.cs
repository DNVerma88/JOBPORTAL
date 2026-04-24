using JobPortal.Application.Common.Interfaces;
using JobPortal.Infrastructure.Hubs;
using JobPortal.Infrastructure.Identity;
using JobPortal.Infrastructure.Persistence;
using JobPortal.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace JobPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Database ──────────────────────────────────────────────────────
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsql.EnableRetryOnFailure(maxRetryCount: 3);
                    npgsql.CommandTimeout(60);
                });
        });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // HttpContextAccessor for user/tenant resolution
        services.AddHttpContextAccessor();

        // ── Services ──────────────────────────────────────────────────────
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        services.AddScoped<ICacheService, RedisCacheService>();

        // ── SignalR ───────────────────────────────────────────────────────
        var redisConnectionString = configuration.GetConnectionString("Redis");
        var signalRBuilder = services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = false; // disable in production
        });

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            signalRBuilder.AddStackExchangeRedis(redisConnectionString, options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal("JobPortal");
            });
        }

        services.AddSingleton<IUserIdProvider, NotificationUserIdProvider>();

        // ── Redis Cache ───────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "JobPortal:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache(); // fallback for dev
        }

        return services;
    }
}
