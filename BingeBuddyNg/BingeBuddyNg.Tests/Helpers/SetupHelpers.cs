﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Persistence;
using BingeBuddyNg.Services.User.Queries;
using Moq;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Tests.Helpers
{
    internal static class SetupHelpers
    {
        internal static SearchUsersQuery SetupSearchUsersQuery(IEnumerable<string> testUserIds)
        {
            var storageAccessServiceMock = new Mock<IStorageAccessService>();

            var query = new SearchUsersQuery(storageAccessServiceMock.Object);

            storageAccessServiceMock
                .Setup(s => s.QueryTableAsync<JsonTableEntity<UserEntity>>(TableNames.Users, It.IsAny<string>()))
                .ReturnsAsync(testUserIds.Select(u => new JsonTableEntity<UserEntity>(StaticPartitionKeys.User, u,
                    new UserEntity() { Id = u, Name = "username", PushInfo = new PushInfo("sub", "auth", "p256dh") })).ToList());
            return query;
        }
    }
}