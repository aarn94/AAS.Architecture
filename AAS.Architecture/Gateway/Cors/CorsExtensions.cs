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
        
        public static IConveyBuilder AddCors(this IConveyBuilder builder, string corsName = "SecurityPolicy", string corsSectionName = "cors")
        {
            var webOptions = builder.GetOptions<CorsOptions>(corsSectionName);
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsName,
                    builder =>
                    {
                        builder
                            .WithOrigins(webOptions.Domains)
                            .WithMethods(webOptions.Methods)
                            .WithExposedHeaders(webOptions.ExposedHeaders)
                            .WithHeaders(webOptions.Headers);
                    });
            });

            return builder;
        }

        public static IApplicationBuilder UseWebCors(this IApplicationBuilder builder, string corsName = "AllowAll") => builder.UseCors(corsName);
        
        public static IApplicationBuilder UseCors(this IApplicationBuilder builder, string corsName = "SecurityPolicy") => builder.UseCors(corsName);
    }
}