using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LeaderboardAPI.Interfaces;
using LeaderboardAPI.Models;

namespace LeaderboardAPI.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IDatabase _data;

        public LeaderboardService(IDatabase data)
        {
            _data = data;
        }

        public async Task<List<LeaderboardRowDTO>> Get(long? clientId)
        {
            return await _data.Get(clientId);
        }

        public async Task Create(LeaderboardRowDTO rowIn)
        {
            await _data.Create(rowIn);
        }

        public async Task Update(LeaderboardRowDTO rowIn)
        {
            await _data.Update(rowIn);
        }

        public async Task Delete(LeaderboardRowDTO rowIn)
        {
            await _data.Delete(rowIn);
        }

        public async Task Delete(long clientId)
        {
            await _data.Delete(clientId);
        }

        public async Task DeleteAll()
        {
            await _data.DeleteAll();
        }
    }
}