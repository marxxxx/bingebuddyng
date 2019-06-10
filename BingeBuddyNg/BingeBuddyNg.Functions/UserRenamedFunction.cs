using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class UserRenamedFunction
    {
        public IUserRepository UserRepository { get; }

        public UserRenamedFunction(IUserRepository userRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [FunctionName("UserRenamedFunction")]
        public async Task Run([QueueTrigger(QueueNames.UserRenamed, Connection = "AzureWebJobsStorage")]string message, ILogger log)
        {
            var userRenamedMessage = JsonConvert.DeserializeObject<UserRenamedMessage>(message);

            var user = await this.UserRepository.FindUserAsync(userRenamedMessage.UserId);
            if(user == null)
            {
                log.LogWarning($"User [{userRenamedMessage.UserId}] not found.");
                return;
            }

            foreach(var friendUserInfo in user.Friends)
            {
                var friendUser = await this.UserRepository.FindUserAsync(friendUserInfo.UserId);
                if(friendUser != null)
                {
                    var foundFriend = friendUser.Friends.FirstOrDefault(f => f.UserId == userRenamedMessage.UserId);
                    if(foundFriend != null)
                    {
                        foundFriend.UserName = userRenamedMessage.NewUserName;
                        await this.UserRepository.UpdateUserAsync(friendUser);
                    }
                }
            }
        }
    }
}
