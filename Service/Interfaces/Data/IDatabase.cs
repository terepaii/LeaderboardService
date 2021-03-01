using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace LeaderboardAPI.Interfaces
{
    public interface IDatabase
    {
        public Task<List<LeaderboardRowDTO>> Get(Guid? clientId);

        public Task Create(LeaderboardRowDTO rowIn);

        public Task Update(LeaderboardRowDTO rowIn);

        public Task Delete(LeaderboardRowDTO rowIn);

        public Task Delete(Guid clientId);

        public Task DeleteAll();
    }
}