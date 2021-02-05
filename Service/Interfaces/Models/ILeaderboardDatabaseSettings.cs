  
namespace LeaderboardAPI.Interfaces
{
    public interface ILeaderboardDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}