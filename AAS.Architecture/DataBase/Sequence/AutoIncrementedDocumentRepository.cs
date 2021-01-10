using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AAS.Architecture.DataBase.Sequence
{
    public abstract class AutoIncrementedDocumentRepository
    {
        protected readonly IMongoDatabase Database;
        private readonly string sequenceTableName;

        protected AutoIncrementedDocumentRepository(IMongoDatabase database, string sequenceTableName)
        {
            Database = database;
            this.sequenceTableName = sequenceTableName;
        }
        
        protected async Task<long> GetNextSequenceValue(string tableName)
        {
            var collection = Database.GetCollection<SequenceDocument>(sequenceTableName);
            var filter = Builders<SequenceDocument>.Filter.Eq(a => a.Name, tableName);
            var update =
                Builders<SequenceDocument>.Update.Inc(a => a.Value, 1);
            update = update.SetOnInsert("_id", 
                Guid.NewGuid().ToString());

            var sequence = await collection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<SequenceDocument, SequenceDocument> { IsUpsert = true, ReturnDocument = ReturnDocument.After });
            
           return sequence.Value;
        }
    }
}