using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class UserRenamedFunction
    {
        private readonly IUserRepository userRepository;

        public UserRenamedFunction(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [FunctionName(nameof(UserRenamedFunction))]
        public async Task Run([QueueTrigger(QueueNames.UserRenamed, Connection = "AzureWebJobsStorage")] UserRenamedMessage userRenamedMessage, ILogger log)
        {
            var user = await this.userRepository.GetUserAsync(userRenamedMessage.UserId);

            foreach (var friendUserInfo in user.Friends)
            {
                var friendUser = await this.userRepository.GetUserAsync(friendUserInfo.UserId);

                var foundFriend = friendUser.Friends.FirstOrDefault(f => f.UserId == userRenamedMessage.UserId);
                if (foundFriend != null)
                {
                    foundFriend.UserName = userRenamedMessage.NewUserName;
                    await this.userRepository.UpdateUserAsync(friendUser.ToEntity());
                }
            }
        }
    }
}
