using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Invitation.Persistence
{
    public class InvitationTableEntity : TableEntity
    {
        public string InvitationToken { get; set; }
        public string InvitingUserId { get; set; }

        public InvitationTableEntity()
        { }

        public InvitationTableEntity(string partitionKey, string invitationToken, string invitingUserId)
            :base(partitionKey, invitationToken)
        {
            this.InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
            this.InvitingUserId = invitingUserId ?? throw new ArgumentNullException(nameof(invitingUserId));
        }
    }
}
