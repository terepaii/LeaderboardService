using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LeaderboardAPI.Interfaces;

namespace LeaderboardAPI.Interfaces
{
    public interface ILeaderboardService
    {
        public Task<LeaderboardRowDTO> Get(Guid clientId, short leaderboardId);

        public Task<List<LeaderboardRowDTO>> GetRowsPaginated(short leaderboardId, int offset, int limit);

        public Task Create(LeaderboardRowDTO rowIn);

        public Task Update(LeaderboardRowDTO rowIn);

        public Task Delete(LeaderboardRowDTO rowIn);

        public Task Delete(Guid clientId, short leaderboardId);

        public Task DeleteAll();
    }
}