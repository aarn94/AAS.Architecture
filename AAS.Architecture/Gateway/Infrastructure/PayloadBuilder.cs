using System.IO;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AAS.Architecture.Gateway.Infrastructure
{
    internal sealed class PayloadBuilder : IPayloadBuilder
    {
        public async Task<T> BuildFromJsonAsync<T>(HttpRequest request) where T : class, new()
        {
            if (request.Body is null)
            {
                return new T();
            }

            using var reader = new StreamReader(request.Body);
            var content = await reader.ReadToEndAsync().WithoutCapturingContext();
            return string.IsNullOrWhiteSpace(content) ? new T() : JsonConvert.DeserializeObject<T>(content);
        }
    }
}