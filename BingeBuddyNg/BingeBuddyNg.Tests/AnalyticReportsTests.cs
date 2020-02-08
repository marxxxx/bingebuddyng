using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.Statistics.Querys;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class AnalyticReportsTests
    {
        [Fact]
        public async Task GetPersonalUsagePerWeekdayQuery_fills_gaps_and_sorts_result()
        {
            // Arrange
            var storageAccessServiceMock = new Mock<IStorageAccessService>();
            storageAccessServiceMock.Setup(s => s.QueryTableAsync<PersonalUsagePerWeekdayTableEntity>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<PersonalUsagePerWeekdayTableEntity>()
                {
                    new PersonalUsagePerWeekdayTableEntity() { weekDay = "Fri"},
                    new PersonalUsagePerWeekdayTableEntity() { weekDay = "Sat"}
                });

            var queryHandler = new AnalyticReportsQueryHandler(storageAccessServiceMock.Object);

            // Act
            var result = await queryHandler.Handle(new GetPersonalUsagePerWeekdayQuery("123"), new CancellationTokenSource().Token);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(7, result.Count());
            Assert.Collection(result,
                r => Assert.Equal("Mon", r.WeekDay),
                r => Assert.Equal("Tue", r.WeekDay),
                r => Assert.Equal("Wed", r.WeekDay),
                r => Assert.Equal("Thu", r.WeekDay),
                r => Assert.Equal("Fri", r.WeekDay),
                r => Assert.Equal("Sat", r.WeekDay),
                r => Assert.Equal("Sun", r.WeekDay)
                );
        }
    }
}
