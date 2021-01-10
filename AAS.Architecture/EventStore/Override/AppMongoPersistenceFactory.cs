using System;
using NEventStore.Persistence;
using NEventStore.Persistence.MongoDB;
using NEventStore.Serialization;

namespace AAS.Architecture.EventStore.Override
{
    internal sealed class AppMongoPersistenceFactory : MongoPersistenceFactory
    {
        private readonly Func<string> connectionStringProvider;
        private readonly AppMongoPersistenceOptions options;
        private readonly IDocumentSerializer serializer;

        public AppMongoPersistenceFactory(
            Func<string> connectionStringProvider,
            IDocumentSerializer serializer,
            AppMongoPersistenceOptions options = null) : base(connectionStringProvider, serializer, options)
        {
            this.connectionStringProvider = connectionStringProvider;
            this.serializer = serializer;
            this.options = options ?? new AppMongoPersistenceOptions();
        }

        public override IPersistStreams Build() =>
            new MongoPersistenceEngine(options.ConnectToDatabase(connectionStringProvider()), serializer, options);
    }
}