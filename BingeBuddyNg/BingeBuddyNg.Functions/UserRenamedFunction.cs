using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        public async Task Run([QueueTrigger(QueueNames.UserRenamed, Connection = "AzureWebJobsStorage")]string message, ILogger log)
        {
            var userRenamedMessage = JsonConvert.DeserializeObject<UserRenamedMessage>(message);

            var user = await this.userRepository.FindUserAsync(userRenamedMessage.UserId);
            if(user == null)
            {
                log.LogWarning($"User [{userRenamedMessage.UserId}] not found.");
                return;
            }

            foreach(var friendUserInfo in user.Friends)
            {
                var friendUser = await this.userRepository.FindUserAsync(friendUserInfo.UserId);
                if(friendUser != null)
                {
                    var foundFriend = friendUser.Friends.FirstOrDefault(f => f.UserId == userRenamedMessage.UserId);
                    if(foundFriend != null)
                    {
                        foundFriend.UserName = userRenamedMessage.NewUserName;
                        await this.userRepository.UpdateUserAsync(friendUser);
                    }
                }
            }
        }
    }
}
