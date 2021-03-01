using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LeaderboardAPI.Interfaces;
using LeaderboardAPI.Models;
namespace LeaderboardAPI.Data
{
        public class LeaderboardDatabaseMongoDB : IDatabase
        {
        private readonly IMongoCollection<LeaderboardRowMongoDB> _rows;
        private readonly LeaderboardMongoDBSettings _settings;

        public LeaderboardDatabaseMongoDB(IOptions<LeaderboardMongoDBSettings> options)
        {
            _settings = options.Value;
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);
            _rows = database.GetCollection<LeaderboardRowMongoDB>(_settings.LeaderboardCollectionName);

            var _key = Builders<LeaderboardRowMongoDB>.IndexKeys.Descending("Rating");
            _rows.Indexes.CreateOne(new CreateIndexModel<LeaderboardRowMongoDB>(_key));
        }

        public async Task<List<LeaderboardRowDTO>> Get(Guid? clientId)
        {
            List<LeaderboardRowDTO> rows = new List<LeaderboardRowDTO>();

            var filter = clientId == null ?                                                  // Is the client id null?
                         Builders<LeaderboardRowMongoDB>.Filter.Empty :                      // If so, create a filter to retrieve all documents
                         Builders<LeaderboardRowMongoDB>.Filter.Eq("ClientId", clientId);    // If not, look for a particular document with the client id

            
            // Always return results sorted in descending order
            var sortOption = new FindOptions<LeaderboardRowMongoDB>
            {
                Sort = Builders<LeaderboardRowMongoDB>.Sort.Descending("Rating")
            };

            using (var cursor = await _rows.FindAsync(filter, sortOption))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (LeaderboardRowMongoDB row in batch)
                    {
                        rows.Add(new LeaderboardRowDTO
                        {
                            ClientId = row.ClientId,
                            Rating = row.Rating
                        });
                    }
                }
            }
            return rows;   
        }

        public async Task Create(LeaderboardRowDTO rowIn)
        {
            await _rows.InsertOneAsync(new LeaderboardRowMongoDB(rowIn));
        }

        public async Task Update(LeaderboardRowDTO rowIn)
        {
            var filter = Builders<LeaderboardRowMongoDB>.Filter.Eq("ClientId", rowIn.ClientId);
            var update = Builders<LeaderboardRowMongoDB>.Update.Set("Rating", rowIn.Rating);
            await _rows.UpdateOneAsync(filter, update);
        }

        public async Task Delete(LeaderboardRowDTO rowIn)
        {
            await _rows.DeleteOneAsync(row => row.ClientId == rowIn.ClientId);
        }

        public async Task Delete(Guid clientId)
        {
            await _rows.DeleteOneAsync(row => row.ClientId.Equals(clientId));
        }

        public async Task DeleteAll()
        {
            await _rows.DeleteManyAsync(Builders<LeaderboardRowMongoDB>.Filter.Empty);
        }
    }
}