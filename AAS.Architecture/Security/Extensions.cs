using Microsoft.AspNetCore.Builder;
using Convey.Auth;

namespace AAS.Architecture.Security
{
    public static class Extensions
    {
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app) =>
            app.UseAuthentication()
                .UseAuthorization()
                .UseAccessTokenValidator();
    }
}