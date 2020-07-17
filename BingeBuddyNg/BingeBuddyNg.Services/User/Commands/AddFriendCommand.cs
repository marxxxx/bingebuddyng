using System;
using System.Threading.Tasks;

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
            var user = await this.userRepository.GetUserAsync(userId);
            var friend = await this.userRepository.GetUserAsync(friendUserId);

            user.AcceptFriendRequest(friend.ToUserInfo());
            friend.AcceptFriendRequest(user.ToUserInfo());

            await Task.WhenAll(userRepository.UpdateUserAsync(user.ToEntity()), userRepository.UpdateUserAsync(friend.ToEntity()));
        }
    }
}
