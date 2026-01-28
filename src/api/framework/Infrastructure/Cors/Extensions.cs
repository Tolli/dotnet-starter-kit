using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Infrastructure.Cors;
public static class Extensions
{
    private const string CorsPolicy = nameof(CorsPolicy);
    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
    {
        var corsOptions = config.GetSection(nameof(CorsOptions)).Get<CorsOptions>();
        if (corsOptions == null) { return services; }
        Console.WriteLine("CORS Origins: " + string.Join(", ", corsOptions.AllowedOrigins));
        return services.AddCors(opt =>
        opt.AddPolicy(CorsPolicy, policy =>
            policy.AllowAnyHeader()
                .WithHeaders("tenant", "content-type", "authorization", "accept", "accept-language", "x-requested-with")
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("tenant", "Content-Disposition")
                .WithOrigins(corsOptions.AllowedOrigins.ToArray())));
    }

    internal static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors(CorsPolicy);
    }
}
