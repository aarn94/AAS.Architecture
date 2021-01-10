using Convey;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AAS.Architecture.Gateway.Cors
{
    public static class CorsExtensions
    {
        private const string WebStorageSectionName = "web";
        
        public static IConveyBuilder AddWebCors(this IConveyBuilder builder, string corsName = "AllowAll")
        {
            var webOptions = builder.GetOptions<WebOptions>(WebStorageSectionName);
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsName,
                    builder =>
                    {
                        builder
                            .WithOrigins(webOptions.Url)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            return builder;
        }

        public static IApplicationBuilder UseWebCors(this IApplicationBuilder builder, string corsName = "AllowAll") => builder.UseCors(corsName);
    }
}