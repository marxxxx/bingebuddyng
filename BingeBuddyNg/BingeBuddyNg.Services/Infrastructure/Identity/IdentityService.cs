using System;
using Microsoft.AspNetCore.Http;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class IdentityService : IIdentityService
    {
        private const string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public IHttpContextAccessor HttpContextAccessor { get; }

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetCurrentUserId()
        {
            var subClaim = this.HttpContextAccessor.HttpContext.User.FindFirst(NameIdentifier);
            return subClaim?.Value;
        }
    }
}
