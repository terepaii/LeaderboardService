using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LeaderboardAPI.Interfaces;

namespace LeaderboardAPI.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IDatabase _data;

        public LeaderboardService(IDatabase data)
        {
            _data = data;
        }

        public async Task<LeaderboardRowDTO> Get(Guid clientId, short leaderboardId)
        {
            return await _data.Get(clientId, leaderboardId);
        }

        public async Task<List<LeaderboardRowDTO>> GetRowsPaginated(short leaderboardId, int offset, int limit)
        {
            return await _data.GetRowsPaginated(leaderboardId, offset, limit);
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

        public async Task Delete(Guid clientId, short leaderboardId)
        {
            await _data.Delete(clientId, leaderboardId);
        }

        public async Task DeleteAll()
        {
            await _data.DeleteAll();
        }
    }
}