using MongoDB.Driver;
using NEventStore.Persistence.MongoDB;

namespace AAS.Architecture.EventStore.Override
{
    internal sealed class AppMongoPersistenceOptions : MongoPersistenceOptions
    {
        public override IMongoDatabase ConnectToDatabase(string connectionString)
        {
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            return new MongoClient(
                    $"mongodb://{mongoUrlBuilder.Username}:{mongoUrlBuilder.Password}@{mongoUrlBuilder.Server}")
                .GetDatabase(mongoUrlBuilder.DatabaseName);
        }
    }
}