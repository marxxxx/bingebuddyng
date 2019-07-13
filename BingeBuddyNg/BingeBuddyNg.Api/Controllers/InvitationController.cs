using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Invitation;
using BingeBuddyNg.Services.Invitation.Commands;
using BingeBuddyNg.Services.Invitation.Querys;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class InvitationController : Controller
    {
        public IIdentityService IdentityService { get; }
        public IMediator Mediator { get; }
        
        public InvitationController(IIdentityService identityService, IMediator mediator)
        {
            IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [AllowAnonymous]
        [HttpGet("{invitationToken}")]
        public async Task<InvitationDTO> GetInvitation(string invitationToken)
        {
            var result = await Mediator.Send(new GetInvitationQuery(invitationToken));
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> CreateInvitation()
        {
            var userId = IdentityService.GetCurrentUserId();
            var invitationToken = await Mediator.Send(new CreateInvitationCommand(userId));
            return Json(invitationToken);
        }

        [HttpPut("{invitationToken}")]
        public async Task AcceptInvitation(string invitationToken)
        {
            var userId = IdentityService.GetCurrentUserId();
            await Mediator.Send(new AcceptInvitationCommand(userId, invitationToken));
        }
        
    }
}
