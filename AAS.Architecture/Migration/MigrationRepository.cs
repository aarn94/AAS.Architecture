using System;
using System.Linq;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using Convey.Persistence.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AAS.Architecture.Migration
{
    internal sealed class MigrationRepository : IMigrationRepository
    {
        private readonly IMongoRepository<MigrationDocument, Guid> repository;

        public MigrationRepository(IMongoRepository<MigrationDocument, Guid> repository) => this.repository = repository;

        public async Task<int> GetMaxVersionAsync()
        {
            var any = await repository.FindAsync((e) => true).WithoutCapturingContext();
            if (any.Count == 0)
                return 0;
            var maxVersion = await repository.Collection.AsQueryable().MaxAsync(e => e.Version).WithoutCapturingContext();
            return maxVersion;
        }

        public Task AddMigrationAsync(int version, string name) => repository.AddAsync(
            new MigrationDocument
            {
                Id = Guid.NewGuid(),
                Name = name,
                Version = version
            });
    }
}