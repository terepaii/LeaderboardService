namespace LeaderboardApi.Models
{
    public interface ILeaderboardDatabaseSettings
    {
        string LeaderboardCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
    
    public class LeaderboardDatabaseSettings : ILeaderboardDatabaseSettings
    {
        public string LeaderboardCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}