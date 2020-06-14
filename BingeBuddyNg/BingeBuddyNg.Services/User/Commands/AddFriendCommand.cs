using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Core.User.Commands
{
    public class AddFriendCommand
    {
        private readonly IUserRepository userRepository;

        public AddFriendCommand(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task ExecuteAsync(string userId, string friendUserId)
        {
            var user = await this.userRepository.FindUserAsync(userId);
            var friend = await this.userRepository.FindUserAsync(friendUserId);

            user.AddFriend(friend.ToUserInfo());
            friend.AddFriend(user.ToUserInfo());

            await Task.WhenAll(userRepository.UpdateUserAsync(user.ToEntity()), userRepository.UpdateUserAsync(friend.ToEntity()));
        }
    }
}
