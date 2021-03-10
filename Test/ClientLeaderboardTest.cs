using Microsoft.AspNetCore.Mvc;
using System;
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
                ClientId = Guid.NewGuid(),
                Rating = 1,
                LeaderboardId = 1
            };
            var result = Validator.TryValidateObject(row, new ValidationContext(row), null, true);

            Assert.True(result);
        }

        [Fact]
        public void LeaderboardRowNegativeRatingTest()
        {
            var row = new LeaderboardRowDTO
            {
                ClientId = Guid.NewGuid(),
                Rating = -1,
                LeaderboardId = 1
            };
            var result = Validator.TryValidateObject(row, new ValidationContext(row), null, true);

            Assert.False(result);
        }

        // TODO: Validate Guid Leaderboard Row Test

        [Fact]
        public void GetRowTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var clientId = Guid.NewGuid();
            long rating = 1;
            short leaderboardId = 1;
            var expectedResult = new LeaderboardRowDTO{
                    ClientId = clientId,
                    Rating = rating,
                    LeaderboardId = leaderboardId
                };
            leaderboardService.Setup(m => m.Get(clientId, leaderboardId)).Returns(Task.FromResult(expectedResult));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.Get(clientId, leaderboardId).Result.Value;
            Assert.True(AssertRowEqual(result, expectedResult));
        }

        [Fact]
        public void GetRowEmptyTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var clientId = Guid.NewGuid();
            short leaderboardId = 1;
            leaderboardService.Setup(m => m.Get(clientId, leaderboardId)).Returns(Task.FromResult((LeaderboardRowDTO)null));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.Get(clientId, leaderboardId).Result.Result;
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetRowsPaginatedTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            
            short leaderboardId = 1;
            int offset = 0;
            int limit = 2;
            leaderboardService.Setup(m => m.GetRowsPaginated(leaderboardId, offset, limit)).Returns(Task.FromResult(new List<LeaderboardRowDTO>()
            {
                new LeaderboardRowDTO{
                    ClientId = Guid.NewGuid(),
                    Rating = 2,
                    LeaderboardId = leaderboardId
                },
                new LeaderboardRowDTO{
                    ClientId = Guid.NewGuid(),
                    Rating = 1,
                    LeaderboardId = leaderboardId
                }
            }));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.GetRowsPaginated(leaderboardId, offset, limit).Result.Value;
            Assert.Equal(result.Count, 2);
            Assert.True(result.TrueForAll(p => p.LeaderboardId == leaderboardId));
        }

        [Fact]
        public void GetLeaderboardRowClientDoesNotExistTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var clientId = Guid.NewGuid();
            short leaderboardId = 1;
            leaderboardService.Setup(m => m.Get(clientId, leaderboardId)).Returns(Task.FromResult((LeaderboardRowDTO)null));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.Get(clientId, leaderboardId).Result.Result;
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PostRowTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var row = new LeaderboardRowDTO
            {
                ClientId = Guid.NewGuid(),
                Rating = 1,
                LeaderboardId = 1
            };
            leaderboardService.Setup(m => m.Get(row.ClientId, row.LeaderboardId)).Returns(Task.FromResult((LeaderboardRowDTO)null));
            leaderboardService.Setup(m => m.Create(row)).Returns(Task.CompletedTask);

            var clientController = new ClientController(leaderboardService.Object);

            var postResult = clientController.Post(row).Result;
            Assert.IsType<OkResult>(postResult);

            // Assert the result is there
            leaderboardService.Setup(m => m.Get(row.ClientId, row.LeaderboardId)).Returns(Task.FromResult( row ));

            var getResult = clientController.Get(row.ClientId, row.LeaderboardId).Result.Value;
            Assert.True(AssertRowEqual(row, getResult));
        }

        [Fact]
        public void PostRowAlreadyExistsTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var row = new LeaderboardRowDTO
            {
                ClientId = Guid.NewGuid(),
                Rating = 1,
                LeaderboardId = 1
            };
            leaderboardService.Setup(m => m.Get(row.ClientId, row.LeaderboardId)).Returns(Task.FromResult( row ));

            var clientController = new ClientController(leaderboardService.Object);

            var result = clientController.Post(row).Result;
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public void UpdateLeaderboardRowTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var clientId = Guid.NewGuid();
            var existingRow = new LeaderboardRowDTO
            {
                ClientId = clientId,
                Rating = 1,
                LeaderboardId = 1
            };
            var row = new LeaderboardRowDTO
            {
                ClientId = clientId,
                Rating = 2,
                LeaderboardId = 1
            };

            leaderboardService.Setup(m => m.Get(existingRow.ClientId, existingRow.LeaderboardId)).Returns(Task.FromResult( existingRow ));
            leaderboardService.Setup(m => m.Update(row)).Returns(Task.CompletedTask);

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Update(row).Result;
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void UpdateRowDoesNotExistTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();
            var row = new LeaderboardRowDTO
            {
                ClientId = Guid.NewGuid(),
                Rating = 2,
                LeaderboardId = 1
            };

            leaderboardService.Setup(m => m.Get(row.ClientId, row.LeaderboardId)).Returns(Task.FromResult((LeaderboardRowDTO)null));

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Update(row).Result;
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteLeaderboardRowTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();

            var clientId = Guid.NewGuid();
            short leaderboardId = 1;
            leaderboardService.Setup(m => m.Get(clientId, leaderboardId)).Returns(Task.FromResult(
                new LeaderboardRowDTO
                {
                    ClientId = clientId,
                    Rating = 1,
                    LeaderboardId = leaderboardId
                }
             ));
             leaderboardService.Setup(m => m.Delete(clientId, leaderboardId)).Returns(Task.CompletedTask);

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Delete(clientId, leaderboardId).Result;
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteLeaderboardRowDoesNotExistTest()
        {
            var leaderboardService = new Mock<ILeaderboardService>();

            var clientId = Guid.NewGuid();
            short leaderboardId = 1;
            leaderboardService.Setup(m => m.Get(clientId, leaderboardId)).Returns(Task.FromResult((LeaderboardRowDTO)null));

            var clientController = new ClientController(leaderboardService.Object);
            
            var result = clientController.Delete(clientId, leaderboardId).Result;
            Assert.IsType<NotFoundResult>(result);
        }

        private bool AssertRowEqual(LeaderboardRowDTO rowA, LeaderboardRowDTO rowB)
        {
            return rowA.ClientId == rowB.ClientId && 
                   rowA.Rating == rowB.Rating &&
                   rowA.LeaderboardId == rowB.LeaderboardId; 
        }
    }
}
