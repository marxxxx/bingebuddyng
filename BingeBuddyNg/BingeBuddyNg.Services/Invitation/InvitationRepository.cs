using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Invitation.Persistence;
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

        public async Task CreateAsync(Guid invitationToken, string userId)
        {
            var entity = new InvitationTableEntity(StaticPartitionKeys.Invitation, invitationToken.ToString(), userId);
            await this.storageAccess.InsertAsync(TableNames.Invitations, entity);
        }

        public async Task<InvitationTableEntity> GetAsync(Guid invitationToken)
        {
            var invitationEntity = await storageAccess.GetTableEntityAsync<InvitationTableEntity>(TableNames.Invitations, StaticPartitionKeys.Invitation, invitationToken.ToString());
            if (invitationEntity == null)
            {
                throw new NotFoundException($"Invitation [{invitationToken}] not found!");

            }
            return invitationEntity;
        }
    }
}
