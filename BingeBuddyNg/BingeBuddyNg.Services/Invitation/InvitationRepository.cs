﻿using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Invitation;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Invitation
{
    public class InvitationRepository : IInvitationRepository
    {
        private const string TableName = "invitations";
        private const string PartitionKeyValue = "Invitation";

        private readonly IStorageAccessService storageAccess;

        public InvitationRepository(IStorageAccessService storageAccess)
        {
            this.storageAccess = storageAccess ?? throw new ArgumentNullException(nameof(storageAccess));
        }

        public async Task<string> CreateInvitationAsync(string userId)
        {
            var table = storageAccess.GetTableReference(TableName);

            string invitationToken = Guid.NewGuid().ToString();

            var entity = new InvitationTableEntity(PartitionKeyValue, invitationToken, userId);
            TableOperation saveOperation = TableOperation.Insert(entity);
            
            await table.ExecuteAsync(saveOperation);

            return invitationToken;
        }

        public async Task<Invitation> AcceptInvitationAsync(string userId, string token)
        {
            var table = storageAccess.GetTableReference(TableName);
            var invitationEntity = await FindInvitationEntityAsync(token);
            
            TableOperation operation = TableOperation.Replace(invitationEntity);

            await table.ExecuteAsync(operation);

            var result = new Invitation(invitationEntity.InviationToken, invitationEntity.InvitingUserId);
            return result;
        }

        public async Task<Invitation> GetInvitationAsync(string invitationToken)
        {
            var invitationEntity = await FindInvitationEntityAsync(invitationToken);
            if(invitationEntity == null)
            {
                throw new NotFoundException($"Invitation {invitationToken} not found!");
            }

            var result = new Invitation(invitationToken, invitationEntity.InvitingUserId);
            return result;
            
        }

        private async Task<InvitationTableEntity> FindInvitationEntityAsync(string invitationToken)
        {
            var invitationEntity = await storageAccess.GetTableEntityAsync<InvitationTableEntity>(TableName, PartitionKeyValue, invitationToken);
            return invitationEntity;
        }
    }
}
