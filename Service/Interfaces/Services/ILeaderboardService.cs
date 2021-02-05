using System.Collections.Generic;
using System.Threading.Tasks;

using LeaderboardAPI.Interfaces;

namespace LeaderboardAPI.Interfaces
{
    public interface ILeaderboardService
    {
        public Task<List<LeaderboardRowDTO>> Get(long? clientId);

        public Task Create(LeaderboardRowDTO rowIn);

        public Task Update(LeaderboardRowDTO rowIn);

        public Task Delete(LeaderboardRowDTO rowIn);

        public Task Delete(long clientId);

        public Task DeleteAll();
    }
}