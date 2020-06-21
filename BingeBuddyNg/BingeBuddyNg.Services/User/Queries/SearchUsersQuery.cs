using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Persistence;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.User.Queries
{
    public class SearchUsersQuery
    {
        private readonly IStorageAccessService storageAccessService;

        public SearchUsersQuery(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<List<UserEntity>> ExecuteAsync(IEnumerable<string> userIds = null, string filterText = null)
        {
            string whereClause = BuildWhereClause(userIds);

            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<UserEntity>>(TableNames.Users, whereClause);

            var users = result.OrderByDescending(u => u.Timestamp).Select(r =>
            {
                var user = r.Entity;
                user.LastOnline = r.Timestamp.UtcDateTime;
                return user;
            }).ToList();

            // TODO: Should soon be improved!
            if (!string.IsNullOrEmpty(filterText))
            {
                string lowerFilter = filterText.ToLower();
                users = users.Where(u => u.Name.ToLower().Contains(lowerFilter)).ToList();
            }

            return users;
        }

        private string BuildWhereClause(IEnumerable<string> userIds)
        {
            if (userIds == null)
            {
                return null;
            }

            string whereClause = null;

            foreach (var u in userIds)
            {
                string filter = TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, u);
                if (whereClause != null)
                {
                    whereClause = TableQuery.CombineFilters(whereClause, TableOperators.Or, filter);
                }
                else
                {
                    whereClause = filter;
                }
            }

            return whereClause;
        }
    }
}
