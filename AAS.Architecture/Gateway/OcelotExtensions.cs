using AAS.Architecture.Exceptions;
using AAS.Architecture.Gateway.Infrastructure;
using AAS.Architecture.Security;
using Convey;
using Convey.Auth;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Security;
using Convey.Tracing.Jaeger;
using Convey.Types;
using Convey.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

namespace AAS.Architecture.Gateway
{
    public static class OcelotExtensions
    {
        public static IConveyBuilder AddGatewayInfrastructure(this IConveyBuilder builder)
        {
            builder.Services.AddSingleton<IPayloadBuilder, PayloadBuilder>();
            builder.Services.AddSingleton<ICorrelationContextBuilder, CorrelationContextBuilder>();
            builder.Services.AddSingleton<IAnonymousRouteValidator, AnonymousRouteValidator>();
            builder.Services.AddTransient<AsyncRoutesMiddleware>();
            builder.Services.AddTransient<ResourceIdGeneratorMiddleware>();
            builder.AddErrorHandler<DefaultExceptionToResponseMapper>();
            builder.AddRabbitMq()
                .AddJaeger()
                .AddJwt()
                .AddSecurity()
                .AddWebApi();

            return builder;
        }
        
        public static IConveyBuilder AddOcelotInfrastructure(this IConveyBuilder builder)
        {
            builder.Services.AddOcelot()
                .AddPolly()
                .AddDelegatingHandler<CorrelationContextHandler>(true);
            
            using var provider = builder.Services.BuildServiceProvider();
            var configuration = provider.GetService<IConfiguration>();
            builder.Services.Configure<AsyncRoutesOptions>(configuration.GetSection("AsyncRoutes"));
            builder.Services.Configure<AnonymousRoutesOptions>(configuration.GetSection("AnonymousRoutes"));

            return builder;
        }

        public static IApplicationBuilder UseOcelotInfrastructure(this IApplicationBuilder app)
        {
            app.UseMiddleware<AsyncRoutesMiddleware>();
            app.UseMiddleware<ResourceIdGeneratorMiddleware>();
            
            app.UseOcelot(GetOcelotConfiguration()).ConfigureAwait(false).GetAwaiter().GetResult();

            return app;
        }

        public static IApplicationBuilder UseGatewayInfrastruture(this IApplicationBuilder app)
        {
            app.UseConvey();
            app.UseErrorHandler();
            app.UseAuth();
            app.UseRabbitMq();
            app.MapWhen(ctx => ctx.Request.Path == "/", a =>
            {
                a.Use((ctx, next) =>
                {
                    var appOptions = ctx.RequestServices.GetRequiredService<AppOptions>();
                    return ctx.Response.WriteAsync(appOptions.Name);
                });
            });

            return app;
        }
        
        
        private static OcelotPipelineConfiguration GetOcelotConfiguration()
            => new OcelotPipelineConfiguration
            {
                AuthenticationMiddleware = async (context, next) =>
                {
                    if (!context.DownstreamReRoute.IsAuthenticated)
                    {
                        await next.Invoke().ConfigureAwait(false);
                        return;
                    }

                    if (context.HttpContext.RequestServices.GetRequiredService<IAnonymousRouteValidator>()
                        .HasAccess(context.HttpContext.Request.Path))
                    {
                        await next.Invoke().ConfigureAwait(false);
                        return;
                    }

                    var authenticateResult = await context.HttpContext.AuthenticateAsync().ConfigureAwait(false);
                    if (authenticateResult.Succeeded)
                    {
                        context.HttpContext.User = authenticateResult.Principal;
                        await next.Invoke().ConfigureAwait(false);
                        return;
                    }

                    context.Errors.Add(new UnauthenticatedError("Unauthenticated"));
                }
            };
    }
}