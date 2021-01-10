using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AAS.Architecture.Exceptions;
using AAS.Architecture.Services;
using GuardNet;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AAS.Architecture.Security
{
    internal sealed class UserDetailsProvider: IUserDetailsProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRequestContextAccessor requestContextAccessor;

        public UserDetailsProvider(IHttpContextAccessor contextAccessor, IRequestContextAccessor requestContextAccessor)
        {
            this.httpContextAccessor = contextAccessor;
            this.requestContextAccessor = requestContextAccessor;
        }


        public Task<string> GetRequiredUserNameAsync() =>
            GetUserNameAsync() ?? throw new CustomerUnauthenticatedException();

        public async Task<ClaimsPrincipal> GetAsync()
        {
            var context = httpContextAccessor.HttpContext;
            if (!(context is {})) return null;
            var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            return authResult?.Principal ?? null;

        }

        public async Task<string> GetClaimValueAsync(string claim)
        {
            var user = await GetClaims();

            return user?.FirstOrDefault(e => string.Equals(e.Type, claim, System.StringComparison.OrdinalIgnoreCase))?.Value;
        }

        public Task<string> GetTokenAsync() => httpContextAccessor.HttpContext.GetTokenAsync("access_token");

        public Task<string> GetRoleAsync() => GetClaimValueAsync(ClaimTypes.Role);

        public Task<string> GetUserNameAsync() => GetClaimValueAsync(ClaimTypes.Name);

        public Task<string> GetEmailAsync() => GetClaimValueAsync(ClaimTypes.Email);

        public Task<string> GetPhoneNumberAsync() => GetClaimValueAsync(ClaimTypes.MobilePhone);
        public Task<string> GetFirstNameAsync() => GetClaimValueAsync(ClaimTypes.GivenName);

        public Task<string> GetSecondNameAsync() => GetClaimValueAsync(ClaimTypes.Surname);

        public async Task RelayAuthorizationAsync(Dictionary<string, string> headers)
        {
            Guard.NotNull(headers, nameof(headers));

            var token = await GetTokenAsync();
            headers.Add(HeaderNames.Authorization, $"Bearer {token}");
        }

        [ItemCanBeNull]
        private async Task<List<Claim>> GetClaims()
        {
            var context = httpContextAccessor.HttpContext;
            if (context is {})
            {
                var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                return authResult?.Principal?.Claims.ToList();
            }

            return requestContextAccessor.ReceivedContext?.User?.Claims?
                .ToList().Select(e => new Claim(e.Key, e.Value)).ToList();
        }
    }
}