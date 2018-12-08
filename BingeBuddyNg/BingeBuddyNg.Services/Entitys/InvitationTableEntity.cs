using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Entitys
{
    public class InvitationTableEntity : TableEntity
    {
        public string InviationToken { get; set; }
        public string InvitingUserId { get; set; }
        public string AcceptingUserId { get; set; }

        public InvitationTableEntity()
        { }

        public InvitationTableEntity(string partitionKey, string invitationToken, string invitingUserId)
            :base(partitionKey, invitationToken)
        {
            this.InviationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
            this.InvitingUserId = invitingUserId ?? throw new ArgumentNullException(nameof(invitingUserId));
        }
    }
}
