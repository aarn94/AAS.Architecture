using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AAS.Architecture.Security
{
    public interface IUserDetailsProvider
    {
        Task<string> GetRequiredUserNameAsync();
        
        [CanBeNull]
        Task<ClaimsPrincipal> GetAsync();

        Task<string> GetClaimValueAsync([NotNull] string claim);

        Task<string> GetTokenAsync();
        
        Task<string> GetRoleAsync();
        Task<string> GetUserNameAsync();
        Task<string> GetEmailAsync();
        Task<string> GetPhoneNumberAsync();
        Task<string> GetFirstNameAsync();
        Task<string> GetSecondNameAsync();
        Task RelayAuthorizationAsync([NotNull] Dictionary<string, string> headers);
    }
}