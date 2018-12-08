using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class InvitationController : Controller
    {

        public IIdentityService IdentityService { get; }
        public IInvitationRepository InvitationRepository { get; }
        public IInvitationService InvitationService { get; set; }
        
        public InvitationController(IIdentityService identityService, 
            IInvitationService invitationService,
            IInvitationRepository invitationRepository)
        {
            IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            InvitationService = invitationService ?? throw new ArgumentNullException(nameof(InvitationService));
            InvitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        }

        [AllowAnonymous]
        [HttpGet("{invitationToken}")]
        public async Task<InvitationInfo> GetInvitation(string invitationToken)
        {
            var result = await InvitationService.GetInvitationInfoAsync(invitationToken);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> CreateInvitation()
        {
            var userId = IdentityService.GetCurrentUserId();
            var invitationToken = await InvitationRepository.CreateInvitationAsync(userId);
            return Json(invitationToken);
        }

        [HttpPut("{invitationToken}")]
        public async Task AcceptInvitation(string invitationToken)
        {
            var userId = IdentityService.GetCurrentUserId();
            await InvitationService.AcceptInvitationAsync(userId, invitationToken);
        }
        
    }
}
