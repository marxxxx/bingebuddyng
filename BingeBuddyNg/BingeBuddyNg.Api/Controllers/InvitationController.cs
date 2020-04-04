using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Invitation;
using BingeBuddyNg.Services.Invitation.Commands;
using BingeBuddyNg.Services.Invitation.Querys;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class InvitationController : Controller
    {
        private readonly IIdentityService identityService;
        private readonly IMediator mediator;
        
        public InvitationController(IIdentityService identityService, IMediator mediator)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [AllowAnonymous]
        [HttpGet("{invitationToken}")]
        public async Task<InvitationDTO> GetInvitation(string invitationToken)
        {
            var result = await mediator.Send(new GetInvitationQuery(invitationToken));
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> CreateInvitation()
        {
            var userId = identityService.GetCurrentUserId();
            var invitationToken = await mediator.Send(new CreateInvitationCommand(userId));
            return Json(invitationToken);
        }

        [HttpPut("{invitationToken}/accept")]
        public async Task AcceptInvitation(string invitationToken)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new AcceptInvitationCommand(userId, invitationToken));
        }
        
    }
}
