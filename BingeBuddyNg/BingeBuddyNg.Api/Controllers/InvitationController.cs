﻿using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Invitation.Commands;
using BingeBuddyNg.Core.Invitation.DTO;
using BingeBuddyNg.Core.Invitation.Querys;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class InvitationController : Controller
    {
        private readonly IIdentityService identityService;
        private readonly ISender mediator;

        public InvitationController(IIdentityService identityService, ISender mediator)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [AllowAnonymous]
        [HttpGet("{invitationToken}")]
        public async Task<InvitationDTO> GetInvitation(Guid invitationToken)
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
        public async Task AcceptInvitation(Guid invitationToken)
        {
            var userId = identityService.GetCurrentUserId();
            await mediator.Send(new AcceptInvitationCommand(userId, invitationToken));
        }
    }
}