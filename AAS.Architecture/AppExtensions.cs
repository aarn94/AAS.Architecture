using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AAS.Architecture.Communication;
using AAS.Architecture.Communication.Contexts;
using AAS.Architecture.Decorators;
using AAS.Architecture.Decorators.Jaeger;
using AAS.Architecture.Events;
using AAS.Architecture.Extensions;
using AAS.Architecture.Initialization;
using AAS.Architecture.Language;
using AAS.Architecture.Metrics;
using AAS.Architecture.Migration;
using AAS.Architecture.Security;
using AAS.Architecture.Services;
using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.Logging.CQRS;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.Outbox.Mongo;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.AppMetrics;
using Convey.Persistence.MongoDB;
using Convey.Persistence.Redis;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;

namespace AAS.Architecture
{
    public static class AppExtensions
    {
        public static IConveyBuilder AddBaseApplication(this IConveyBuilder builder)
        {
            builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return builder
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddInMemoryEventDispatcher()
                .AddInMemoryCommandDispatcher();
        }

        public static IConveyBuilder AddBaseInfrastructure(this IConveyBuilder builder,
            Assembly[] assembliesToScan, string defaultLanguage, List<string> supportingLanguages,
            string migrationDbName)
        {
            builder.Services.AddMemoryCache();

            builder.Services.AddTransient<IMessageBroker, MessageBroker>();
            builder.Services.AddTransient<IAppContextFactory, AppContextFactory>();
            builder.Services.AddTransient<IEventProcessor, EventProcessor>();
            builder.Services.AddTransient<IUserDetailsProvider, UserDetailsProvider>();
            builder.Services.AddSingleton<IFileReader, FileReader>();
            builder.Services.AddSingleton<IRandomTextGenerator, RandomTextGenerator>();
            builder.Services.AddTransient<IMigrationRepository, MigrationRepository>();
            builder.Services.AddTransient<IMigrator, Migrator>();
            builder.Services.AddScoped<IRequestContextAccessor, RequestContextAccessor>();

            builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
            builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));

            builder.Services.AddTransient(ctx => ctx.GetRequiredService<IAppContextFactory>().Create());
            builder.Services.AddTransient<IAppContextFactory, AppContextFactory>();

            builder.Services.AddHostedService<MetricsJob>();
            builder.Services.AddSingleton<CustomMetricsMiddleware>();

            builder.Services.Scan(s => s.FromAssemblies(assembliesToScan)
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            var conventionPack = new ConventionPack {new CamelCaseElementNameConvention()};
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            builder
                .AddQueryHandlers()
                .AddInMemoryQueryDispatcher()
                .AddHttpClient()
                .AddConsul()
                .AddFabio()
                .AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin())
                .AddMessageOutbox(o => o.AddMongo())
                .AddMongo()
                .AddMongoRepository<MigrationDocument, Guid>(migrationDbName)
                .AddRedis()
                .AddMetrics()
                .AddJaeger()
                .AddJaegerDecorators()
                .AddLanguage(defaultLanguage, supportingLanguages);

            builder.Services.AddHostedService<HostedInitializer>();
            return builder;
        }

        public static IConveyBuilder AddHandlersLogging<T>(this IConveyBuilder builder, Assembly assembly,
            T logTemplateMapper) where T : class, IMessageToLogTemplateMapper
        {
            builder.Services.AddSingleton<IMessageToLogTemplateMapper>(logTemplateMapper);

            return builder
                .AddCommandHandlersLogging(assembly)
                .AddEventHandlersLogging(assembly);
        }


        public static IApplicationBuilder UseBaseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
                .UseJaeger()
                .UseMetrics()
                .UseConvey()
                .UsePublicContracts<ContractAttribute>()
                .UseMiddleware<CustomMetricsMiddleware>()
                .UseLanguage();

            var migrator = app.ApplicationServices.GetService<IMigrator>();
            migrator.MigrateAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            return app;
        }

        public static async Task<string> AuthenticateUsingJwtAsync(this HttpContext context)
        {
            var authentication = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme).WithoutCapturingContext();

            return authentication.Succeeded ? authentication.Principal.Identity.Name : string.Empty;
        }
    }
}