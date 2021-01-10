﻿﻿﻿﻿using Convey.Persistence.MongoDB;
    using JetBrains.Annotations;

    namespace AAS.Architecture.EventStore
{
    [UsedImplicitly]
    internal sealed class MongoDbOptionsBuilder : IMongoDbOptionsBuilder
    {
        private readonly MongoDbOptions options = new MongoDbOptions();

        public IMongoDbOptionsBuilder WithConnectionString(string connectionString)
        {
            options.ConnectionString = connectionString;
            return this;
        }

        public IMongoDbOptionsBuilder WithDatabase(string database)
        {
            options.Database = database;
            return this;
        }

        public IMongoDbOptionsBuilder WithSeed(bool seed)
        {
            options.Seed = seed;
            return this;
        }

        public MongoDbOptions Build() => options;
    }
}