using System;
using System.Transactions;
using NEventStore;
using NEventStore.Logging;
using NEventStore.Serialization;

namespace AAS.Architecture.EventStore.Override
{
    internal sealed class AppMongoPersistenceWireup : PersistenceWireup
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(MongoPersistenceWireup));

        public AppMongoPersistenceWireup(
            Wireup inner,
            Func<string> connectionStringProvider,
            IDocumentSerializer serializer,
            AppMongoPersistenceOptions persistenceOptions)
            : base(inner)
        {
            Logger.Debug("Configuring Mongo persistence engine.");
            if (Container.Resolve<TransactionScopeOption>() != TransactionScopeOption.Suppress)
                Logger.Warn("MongoDB does not participate in transactions using TransactionScope.");
            Container.Register(_ =>
                new AppMongoPersistenceFactory(connectionStringProvider, serializer, persistenceOptions).Build());
        }
    }
}