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

        public async Task<List<Row>> Get(long? clientId)
        {
            List<Row> rows = new List<Row>();

            var filter = clientId == null ?                                // Is the client id null?
                         Builders<Row>.Filter.Eq("", "") :                 // If so, create a filter to retrieve all documents
                         Builders<Row>.Filter.Eq("ClientId", clientId);    // If not, look for a particular document with the client id

            using (var cursor = await _rows.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (Row row in batch)
                    {
                        rows.Add(row);
                    }
                }
            }
            return rows;   
        }

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