using System;
using Convey;
using Convey.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;

namespace AAS.Architecture.EventStore
{
    public static class Extensions
    {
        public static IConveyBuilder AddEventStoreRepository<T>(this IConveyBuilder builder, string sectionName = "mongo") where T : IEventBaseAggregate<Guid>, new()
        {
            if (string.IsNullOrWhiteSpace(sectionName))
                sectionName = "mongo";
            
            var options = builder.GetOptions<MongoDbOptions>(sectionName);
            
            var connectionString = $"{options.ConnectionString}/{options.Database}";
            builder.Services.AddTransient<IEventStoreRepository<T>>(services =>
                new EventStoreRepository<T>(new EventStoreContext(connectionString)));

            return builder;
        }
    }
}