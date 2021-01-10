using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AAS.Architecture.Services
{
    public interface IFileReader
    {
        Task<string> ReadFileAsync([NotNull] string path);

        string GetRelativeServerPath([NotNull] string relativePath);
    }
}