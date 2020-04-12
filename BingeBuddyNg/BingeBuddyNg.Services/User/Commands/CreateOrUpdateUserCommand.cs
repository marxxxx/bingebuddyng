using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using System;

namespace BingeBuddyNg.Services.User.Commands
{
    public class CreateOrUpdateUserCommand : IRequest
    {
        public CreateOrUpdateUserCommand(string userId, string name, string profileImageUrl, PushInfo pushInfo, string language)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ProfileImageUrl = profileImageUrl;
            PushInfo = pushInfo;
            Language = language;
        }

        public string UserId { get; }
        public string Name { get; }
        public string ProfileImageUrl { get; }
        public PushInfo PushInfo { get; }
        public string Language { get; }
    }
}
