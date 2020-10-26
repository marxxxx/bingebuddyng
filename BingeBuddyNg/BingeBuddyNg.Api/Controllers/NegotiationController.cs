using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.SignalR.Management;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class NegotiationController : ControllerBase
    {
        private readonly IServiceManager serviceManager;
        private readonly IIdentityService identityService;

        public NegotiationController(IServiceManager serviceManager, IIdentityService identityService)
        {
            this.serviceManager = serviceManager ?? throw new ArgumentNullException(nameof(serviceManager));
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [HttpPost("{hub}/negotiate")]
        public ActionResult Negotiate(string hub)
        {
            string userId = this.identityService.GetCurrentUserId();
            return new JsonResult(new Dictionary<string, string>()
            {
                { "url", serviceManager.GetClientEndpoint(hub) },
                { "accessToken", serviceManager.GenerateClientAccessToken(hub, userId) }
            });
        }
    }
}