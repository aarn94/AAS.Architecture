using System.Threading.Tasks;

namespace AAS.Architecture.Migration
{
    public interface IMigrator
    {
        Task MigrateAsync();
    }
}