using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Convey.Types;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AAS.Architecture.DataBase
{
    public static class Extensions
    {
        public static Task UpdateManyAsync<T, I>(this IMongoCollection<T> collection, IEnumerable<T> data)
        where T : IIdentifiable<I>
        {
            var updates = new List<WriteModel<T>>();
            var filterBuilder = Builders<T>.Filter;

            foreach (var doc in data)
            {
                var filter = filterBuilder.Where(x => x.Id.Equals(doc.Id));
                updates.Add(new ReplaceOneModel<T>(filter, doc));
            }

            return collection.BulkWriteAsync(updates);
        }
        
        public static IMongoQueryable<T> Page<T>(this IMongoQueryable<T> source, int take, int skip) => source.Any() ? source.Skip(skip).Take(take) : source;

        public static ValueTask<D> MaxWithDefaultAsync<T, D>(this IMongoQueryable<T> source, Expression<Func<T, D>> selector, D def) => !source.Any() ? new ValueTask<D>(def) : new ValueTask<D>(source.MaxAsync(selector));
    }
    
    
}