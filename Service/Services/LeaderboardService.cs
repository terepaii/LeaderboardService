using MongoDB.Bson;
using MongoDB.Driver;
using LeaderboardApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LeaderboardApi.Services
{
    public class LeaderboardService
    {
        private readonly IMongoCollection<Row> _rows;

        public LeaderboardService(ILeaderboardDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            
            _rows = database.GetCollection<Row>(settings.LeaderboardCollectionName);
        }

        public List<Row> Get() =>
            _rows.Find(row => true).ToList();

        public Row Get(long clientId) =>
            _rows.Find(row => row.ClientId == clientId).FirstOrDefault();

        public void Create(Row rowIn)
        {
            _rows.InsertOne(rowIn);

             
        }

        public void Update(Row rowIn)
        {
            var filter = Builders<Row>.Filter.Eq("ClientId", rowIn.ClientId);
            var update = Builders<Row>.Update.Set("Rating", rowIn.Rating);
            _rows.UpdateOne(filter, update);
        }

        public void Delete(Row rowIn)
        {
            _rows.DeleteOne(row => row.ClientId == rowIn.ClientId);
        }

        public void Delete(long clientId)
        {
            _rows.DeleteOne(row => row.ClientId == clientId);
        }

        public void DeleteAll()
        {
            _rows.DeleteMany(Builders<Row>.Filter.Empty);
        }
    }
}