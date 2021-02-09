using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Moq;
using Xunit;

using LeaderboardAPI.Controllers;
using LeaderboardAPI.Interfaces;

namespace Test
{
    
    public class ClientLeaderboardTest
    {
        [Fact]
        public void LeaderboardRowTest()
        {
            var row = new LeaderboardRowDTO
            {
                ClientId = 1,
                Rating = 1
            };
            var result = Validator.TryValidateObject(row, new ValidationContext(row), null, true);

            Assert.True(result);
        }

        [Fact]
        public void LeaderboardRowNegativeRatingTest()
        {
            var row = new LeaderboardRowDTO
            {
                ClientId = 1,
                Rating = -1
            };
            var result = Validator.TryValidateObject(row, new ValidationContext(row), null, true);

            Assert.False(result);
        }

        [Fact]
        public void LeaderboardRowNegativeClientIdTest()
        {
            var row = new LeaderboardRowDTO
            {
                ClientId = -1,
                Rating = 1
            };
            var result = Validator.TryValidateObject(row, new ValidationContext(row), null, true);

            Assert.False(result);
        }

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

            var postResult = clientController.Post(row).Result;
            Assert.IsType<OkResult>(postResult);

            leaderboardService.Setup(m => m.Get(row.ClientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>(){ row }));

            var getResult = clientController.GetRow(row.ClientId).Result.Value;
            Assert.Equal(getResult.Count, 1);
            Assert.True(getResult.Exists(p => p.ClientId == row.ClientId));
        }

        [Fact]
        public void PostLeaderboardRowAlreadyExistsTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var row = new LeaderboardRowDTO
            {
                ClientId = 1,
                Rating = 1
            };
            leaderboardService.Setup(m => m.Get(row.ClientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>(){ row }));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.Post(row).Result;
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public void UpdateLeaderboardRowTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var clientId = 1;
            var existingRow = new LeaderboardRowDTO
            {
                ClientId = clientId,
                Rating = 1
            };
            var row = new LeaderboardRowDTO
            {
                ClientId = clientId,
                Rating = 2
            };

            leaderboardService.Setup(m => m.Get(row.ClientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>(){ existingRow }));
            leaderboardService.Setup(m => m.Update(row)).Returns(Task.CompletedTask);

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Update(row).Result;
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void UpdateLeaderboardRowDoesNotExistTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var row = new LeaderboardRowDTO
            {
                ClientId = 1,
                Rating = 2
            };

            leaderboardService.Setup(m => m.Get(row.ClientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>(){  }));

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Update(row).Result;
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteLeaderboardRowTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();

            var clientId = 1;
            leaderboardService.Setup(m => m.Get(clientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>(){ 
                new LeaderboardRowDTO
                {
                    ClientId = clientId,
                    Rating = 1
                }
             }));
             leaderboardService.Setup(m => m.Delete(clientId)).Returns(Task.CompletedTask);

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Delete(clientId).Result;
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteLeaderboardRowDoesNotExistTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();

            var clientId = 1;
            leaderboardService.Setup(m => m.Get(clientId)).Returns(Task.FromResult(new List<LeaderboardRowDTO>(){ /* Empty List*/ }));

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Delete(clientId).Result;
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
