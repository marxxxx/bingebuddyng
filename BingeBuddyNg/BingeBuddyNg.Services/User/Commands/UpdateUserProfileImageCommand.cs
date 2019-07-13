using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User.Commands
{
    public class UpdateUserProfileImageCommand : IRequest
    {
        public UpdateUserProfileImageCommand(string userId, IFormFile image)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public string UserId { get; }
        public IFormFile Image { get; }
    }
}
