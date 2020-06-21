using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Invitation;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Invitation
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly IStorageAccessService storageAccess;

        public InvitationRepository(IStorageAccessService storageAccess)
        {
            this.storageAccess = storageAccess ?? throw new ArgumentNullException(nameof(storageAccess));
        }

        public async Task<string> CreateAsync(string userId)
        {
            string invitationToken = Guid.NewGuid().ToString();

            var entity = new InvitationTableEntity(StaticPartitionKeys.Invitation, invitationToken, userId);
            await this.storageAccess.InsertAsync(TableNames.Invitations, entity);

            return invitationToken;
        }

        public async Task<InvitationTableEntity> GetAsync(string invitationToken)
        {
            var invitationEntity = await storageAccess.GetTableEntityAsync<InvitationTableEntity>(TableNames.Invitations, StaticPartitionKeys.Invitation, invitationToken);
            if (invitationEntity == null)
            {
                throw new NotFoundException($"Invitation [{invitationToken}] not found!");

            }
            return invitationEntity;
        }
    }
}
