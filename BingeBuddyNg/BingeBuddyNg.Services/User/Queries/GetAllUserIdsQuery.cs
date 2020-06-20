using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.User.Queries
{
    public class GetAllUserIdsQuery : IGetAllUserIdsQuery
    {
        private readonly IStorageAccessService storageAccessService;

        public GetAllUserIdsQuery(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<IEnumerable<string>> ExecuteAsync()
        {
            return await this.storageAccessService.GetRowKeysAsync(TableNames.Users, StaticPartitionKeys.User);
        }
    }
}
