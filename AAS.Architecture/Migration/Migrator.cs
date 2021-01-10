using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AAS.Architecture.Migration
{
    [UsedImplicitly]
    public class Migrator : IMigrator
    {
        private readonly IEnumerable<IMigration> migrations;
        private readonly IMigrationRepository migrationRepository;
        private readonly ILogger<Migrator> logger;

        public Migrator(IEnumerable<IMigration> migrations, IMigrationRepository migrationRepository, ILogger<Migrator> logger)
        {
            this.migrations = migrations.ToList();
            this.migrationRepository = migrationRepository;
            this.logger = logger;
        }

        public async Task MigrateAsync()
        {
            var version = await migrationRepository.GetMaxVersionAsync().ConfigureAwait(false);
            
            
            logger.LogInformation($"Current max migration version {version}");

            var migrationsToRun = migrations
                .Where(e => e.Version > version)
                .OrderBy(e => e.Version)
                .ToList();
            
            
            foreach (var migration in migrationsToRun)
            {
                logger.LogInformation($"Execute migration {migration.Name} with version {migration.Version}");
                await migrationRepository.AddMigrationAsync(migration.Version, migration.Name).ConfigureAwait(false);
                await migration.ExecuteAsync().ConfigureAwait(false);
            }

            logger.LogInformation(!migrationsToRun.Any() ? $"No required migrations to run" : "Migration finished");
        }
    }
}