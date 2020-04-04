using System;
using Microsoft.AspNetCore.Http;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class IdentityService : IIdentityService
    {
        private const string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        private readonly IHttpContextAccessor httpContextAccessor;

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetCurrentUserId()
        {
            var subClaim = this.httpContextAccessor.HttpContext.User.FindFirst(NameIdentifier);
            return subClaim?.Value;
        }
    }
}
