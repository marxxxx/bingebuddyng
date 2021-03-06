﻿using System.Collections.Generic;
using System.Linq;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Persistence;
using Moq;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Tests.Helpers
{
    internal static class SetupHelpers
    {
        internal static IUserRepository SetupUserRepository(IEnumerable<string> testUserIds)
        {
            var storageAccessServiceMock = new Mock<IStorageAccessService>();

            var query = new UserRepository(storageAccessServiceMock.Object, new Mock<ICacheService>().Object);

            storageAccessServiceMock
                .Setup(s => s.QueryTableAsync<JsonTableEntity<UserEntity>>(TableNames.Users, It.IsAny<string>()))
                .ReturnsAsync(testUserIds.Select(u => new JsonTableEntity<UserEntity>(StaticPartitionKeys.User, u,
                    new UserEntity() { Id = u, Name = "username", PushInfo = new PushInfo("sub", "auth", "p256dh") })).ToList());
            return query;
        }
    }
}