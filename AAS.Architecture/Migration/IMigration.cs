using System.Threading.Tasks;

namespace AAS.Architecture.Migration
{
    public interface IMigration
    {
        int Version { get; }
        string Name { get; }
        Task ExecuteAsync();
    }
}