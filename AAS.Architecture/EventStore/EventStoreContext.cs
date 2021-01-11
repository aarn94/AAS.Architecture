using System.Linq;
using System.Reflection;
using AAS.Architecture.EventStore.Override;
using MongoDB.Bson.Serialization;
using NEventStore;
using NEventStore.Serialization;
using NEventStore.Serialization.Bson;

namespace AAS.Architecture.EventStore
{ 
    internal sealed class EventStoreContext: IEventStoreContext
    {
        private readonly string connectionString;

        public EventStoreContext(string connectionString) => this.connectionString = connectionString;

        public IStoreEvents Events => WireupEventStore(this.connectionString);
        
        private IStoreEvents WireupEventStore(string connectionString) =>
            Wireup.Init()
                .UsingAppMongoPersistence(connectionString, new DocumentObjectSerializer())
                .InitializeStorageEngine()
                .UsingBsonSerialization()
                .Compress()
                .Build();
    }
}