using System.Threading.Tasks;

namespace AAS.Architecture.Initialization
{
    public interface ISingletonInitializerAsync
    {
        Task InitializeAsync();
    }
}