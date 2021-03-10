using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using Xunit;

using LeaderboardAPI.Controllers;
using LeaderboardAPI.Interfaces;

namespace Test
{
    public class AdminLeaderboardTest
    {
        [Fact]
        public void DeleteAllLeaderboardRowsTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var clientId = Guid.NewGuid();
            short leaderboardId = 1;
            leaderboardService.Setup(m => m.Get(clientId, leaderboardId)).Returns(Task.FromResult(
                new LeaderboardRowDTO{
                    ClientId = clientId,
                    Rating = 2,
                    LeaderboardId = 1
                }));

            var adminController = new AdminController(leaderboardService.Object);

            var result = adminController.DeleteAll().Result;
            Assert.IsType<OkResult>(result);
        }
    }   
}