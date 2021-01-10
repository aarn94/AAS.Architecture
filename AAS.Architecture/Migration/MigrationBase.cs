using System;
using System.Threading.Tasks;

namespace AAS.Architecture.Migration
{
    public abstract class MigrationBase : IMigration
    {
        protected readonly IServiceProvider Provider;

        protected MigrationBase(IServiceProvider provider) => this.Provider = provider;

        public abstract int Version { get; }
        public abstract string Name { get; }
        public abstract Task ExecuteAsync();
    }
}