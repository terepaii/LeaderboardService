using LeaderboardAPI.Interfaces;

namespace LeaderboardAPI.Models
{  
    public class LeaderboardMongoDBSettings 
    {
        public string LeaderboardCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}