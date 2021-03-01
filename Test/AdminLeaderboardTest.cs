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
            leaderboardService.Setup(m => m.Get(null)).Returns(Task.FromResult(new List<LeaderboardRowDTO>()
            {
                new LeaderboardRowDTO{
                    ClientId = Guid.NewGuid(),
                    Rating = 2
                },
                new LeaderboardRowDTO {
                    ClientId = Guid.NewGuid(),
                    Rating = 2
                },
                new LeaderboardRowDTO {
                    ClientId = Guid.NewGuid(),
                    Rating = 3
                }
            }));

            var adminController = new AdminController(leaderboardService.Object);

            var result = adminController.DeleteAll().Result;
            Assert.IsType<OkResult>(result);
        }
    }   
}