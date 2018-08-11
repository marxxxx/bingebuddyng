using BingeBuddyNg.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services
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
