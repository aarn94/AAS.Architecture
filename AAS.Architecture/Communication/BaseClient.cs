using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AAS.Architecture.Exceptions;
using AAS.Architecture.Extensions;
using AAS.Architecture.Models;
using AAS.Architecture.Security;
using AAS.Architecture.Services;
using Convey.HTTP;
using GuardNet;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Polly;

namespace AAS.Architecture.Communication
{
    public abstract class BaseClient
    {
        protected readonly IHttpClient Client;
        protected readonly HttpClientOptions options;
        protected readonly IRequestContextAccessor ContextAccessor;
        protected readonly IUserDetailsProvider UserDetailsProvider;
        
        private const string ContextHeaderName = "Correlation-Context";

        protected BaseClient(IHttpClient client, IRequestContextAccessor contextAccessor, IHttpContextAccessor httpContextAccessor, IUserDetailsProvider userDetailsProvider, HttpClientOptions options)
        {
            Client = client;
            ContextAccessor = contextAccessor;
            UserDetailsProvider = userDetailsProvider;
            this.options = options;
        }

        protected async Task<T> GetJsonAsync<T>(string uri, bool allowNull = false, bool relayHeaders = true) where T : class
        {
            if(relayHeaders)
                await SetCorrelationContextAsync().WithoutCapturingContext();

            if (allowNull)
                return await Client.GetAsync<T>(uri).WithoutCapturingContext();

            return await Policy.Handle<ExternalException>()
                .WaitAndRetryAsync(retryCount:options.Retries, sleepDurationProvider:(errorNumber) => TimeSpan.FromMilliseconds(1000 * errorNumber))
                .ExecuteAsync(async () =>
                {
                    var result = await Client.GetAsync<T>(uri);
                    if (result is null)
                        throw new Exception("Result for this action could not be empty");

                    return result;
                });
            
        }

        protected async Task<OperationResult> SendJsonRequestAsync<T>(HttpMethod method,[NotNull] string uri,[NotNull] T request, bool save = true, bool relayHeaders = true) where T: class
        {
            Guard.NotNullOrWhitespace(uri, nameof(uri));
            if(relayHeaders)
                await SetCorrelationContextAsync().WithoutCapturingContext();
            
            var result = await SendRequest().WithoutCapturingContext();
            if (!result.IsSuccessStatusCode)
            {
                var stringResult = await result.Content.ReadAsStringAsync().WithoutCapturingContext();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                if(stringResult.IsNullOrWhiteSpace())
                    return save ? OperationResult.Error(result.ReasonPhrase) : throw new ExternalException(result.ReasonPhrase, "error");
                var errorResponse = JsonSerializer.Deserialize<ExceptionDetails>(stringResult, options);
                if (errorResponse is {})
                    return save ? OperationResult.Error(errorResponse.Reason, errorResponse.Code): throw new ExternalException(errorResponse.Reason, errorResponse.Code);
                else
                    return save ? OperationResult.Error(result.ReasonPhrase) : throw new ExternalException(result.ReasonPhrase, "error");
            }
            
            return OperationResult.Success();

            Task<HttpResponseMessage> SendRequest()
            {
              
                if (method == HttpMethod.Post)
                    return Client.PostAsync(uri, request); 
                if (method == HttpMethod.Put)
                    return Client.PutAsync(uri, request);
                if (method == HttpMethod.Delete)
                    return Client.DeleteAsync(uri);
                if (method == HttpMethod.Patch)
                    return Client.PatchAsync(uri, request);
                
                throw new InvalidOperationException($"Sending {method} not available");
            }
        }

        private async Task SetCorrelationContextAsync()
        {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            var context = ContextAccessor.ContextForSend;
            if (context is {})
            {
                headers.Add(ContextHeaderName, JsonSerializer.Serialize(context));
            }

            await UserDetailsProvider.RelayAuthorizationAsync(headers).WithoutCapturingContext();

            Client.SetHeaders(headers);
        }
    }
}