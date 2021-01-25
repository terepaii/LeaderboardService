using MongoDB.Bson;
using MongoDB.Driver;
using LeaderboardAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LeaderboardAPI.Services
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

        public async Task Create(Row rowIn)
        {
            await _rows.InsertOneAsync(rowIn);
        }

        public async Task Update(Row rowIn)
        {
            var filter = Builders<Row>.Filter.Eq("ClientId", rowIn.ClientId);
            var update = Builders<Row>.Update.Set("Rating", rowIn.Rating);
            await _rows.UpdateOneAsync(filter, update);
        }

        public async Task Delete(Row rowIn)
        {
            await _rows.DeleteOneAsync(row => row.ClientId == rowIn.ClientId);
        }

        public async Task Delete(long clientId)
        {
            await _rows.DeleteOneAsync(row => row.ClientId == clientId);
        }

        public async Task DeleteAll()
        {
            await _rows.DeleteManyAsync(Builders<Row>.Filter.Empty);
        }
    }
}