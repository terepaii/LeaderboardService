using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

using LeaderboardAPI;
using LeaderboardAPI.Controllers;
using LeaderboardAPI.Interfaces;

namespace Test
{
    public class ClientLeaderboardTest
    {
        [Fact]
        public void GetAllLeaderboardRowsTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            leaderboardService.Setup(m => m.Get(null)).Returns(Task.FromResult(new List<LeaderboardRowDTO>()
            {
                new LeaderboardRowDTO{
                    ClientId = 1,
                    Rating = 2
                },
                new LeaderboardRowDTO {
                    ClientId = 2,
                    Rating = 2
                },
                new LeaderboardRowDTO {
                    ClientId = 3,
                    Rating = 3
                }
            }));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.GetAllRows().Result.Value;

            Assert.Equal(result.Count, 3);
        }

        [Fact]
        public void GetAllLeaderboardRowsEmptyTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            leaderboardService.Setup(m => m.Get(null)).Returns(Task.FromResult(new List<LeaderboardRowDTO>()));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.GetAllRows().Result.Result;

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetLeaderboardRowTest()
        {
            var clientId = 1;
            var leaderboardService = new Mock<ILeaderboardService>();
            leaderboardService.Setup(m => m.Get(clientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>()
            {
                new LeaderboardRowDTO{
                    ClientId = 1,
                    Rating = 2
                }
            }));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.GetRow(clientId).Result.Value;

            Assert.Equal(result.Count, 1);
            Assert.True(result.Exists(p => p.ClientId == clientId));
        }

        [Fact]
        public void GetLeaderboardRowClientDoesNotExistTest()
        {
            var clientId = 404;
            var leaderboardService = new Mock<ILeaderboardService>();
            leaderboardService.Setup(m => m.Get(clientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>() { /*Empty List*/ }));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.GetRow(clientId).Result.Result;

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PostLeaderboardRowTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var row = new LeaderboardRowDTO
            {
                ClientId = 1,
                Rating = 1
            };
            leaderboardService.Setup(m => m.Get(row.ClientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>(){ /*Empty List*/}));
            leaderboardService.Setup(m => m.Create(row)).Returns(Task.CompletedTask);

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.Post(row).Result;

            Assert.IsType<OkResult>(result);
        }
    }
}
