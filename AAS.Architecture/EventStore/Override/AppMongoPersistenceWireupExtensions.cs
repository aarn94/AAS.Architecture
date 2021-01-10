using System;
using NEventStore;
using NEventStore.Serialization;

namespace AAS.Architecture.EventStore.Override
{
    internal static class AppMongoPersistenceWireupExtensions
    {
        public static PersistenceWireup UsingAppMongoPersistence(
            this Wireup wireup,
            string connectionString,
            IDocumentSerializer serializer,
            AppMongoPersistenceOptions options = null) =>
            new AppMongoPersistenceWireup(wireup,
                () => { return connectionString; }, serializer, options);

        public static PersistenceWireup UsingAppMongoPersistence(
            this Wireup wireup,
            Func<string> connectionStringProvider,
            IDocumentSerializer serializer,
            AppMongoPersistenceOptions options = null) =>
            new AppMongoPersistenceWireup(wireup, connectionStringProvider, serializer,
                options);
    }
}