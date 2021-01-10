using System.Threading.Tasks;

namespace AAS.Architecture.Migration
{
    public interface IMigrationRepository
    {
        public Task<int> GetMaxVersionAsync();
        public Task AddMigrationAsync(int version, string name);
        
    }
}