using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using GuardNet;
using Microsoft.AspNetCore.Hosting;

namespace AAS.Architecture.Services
{
    internal sealed class FileReader : IFileReader
    {
        private readonly IWebHostEnvironment env;

        public FileReader(IWebHostEnvironment env)
        {
            this.env = env;
        }
        
        public async Task<string> ReadFileAsync(string path)
        {
            Guard.NotNullOrWhitespace(path, nameof(path));

            using var sourceStream = File.Open(path, FileMode.Open);
            var result = new byte[sourceStream.Length];
            await sourceStream.ReadAsync(result, 0, (int) sourceStream.Length).WithoutCapturingContext();

            return Encoding.ASCII.GetString(result);
        }

        public string GetRelativeServerPath(string relativePath)
        {
            Guard.NotNullOrWhitespace(relativePath, nameof(relativePath));
            
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), relativePath);
        }
    }
}