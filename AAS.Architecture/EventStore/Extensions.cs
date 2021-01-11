using System;
using System.Linq;
using System.Reflection;
using Convey;
using Convey.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace AAS.Architecture.EventStore
{
    public static class Extensions
    {
        public static IConveyBuilder AddEventStore(this IConveyBuilder builder, Assembly eventsAssembly)
        {
            var type = typeof(ISyncDomainEvent);
            var types = eventsAssembly
                .GetTypes()
                .Where(t => type.IsAssignableFrom(t))
                .Where(t => t.IsClass);

            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);

            return builder;
        }
        
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