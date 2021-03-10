using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

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

            var _key = Builders<LeaderboardRowMongoDB>.IndexKeys.Combine(
                Builders<LeaderboardRowMongoDB>.IndexKeys.Ascending("LeaderboardId"),
                Builders<LeaderboardRowMongoDB>.IndexKeys.Descending("Rating")
            );
            _rows.Indexes.CreateOne(new CreateIndexModel<LeaderboardRowMongoDB>(_key));
        }

        public async Task<LeaderboardRowDTO> Get(Guid clientId, short leaderboardId)
        {

            var filter = Builders<LeaderboardRowMongoDB>.Filter.And(
                         Builders<LeaderboardRowMongoDB>.Filter.Eq("LeaderboardId", leaderboardId),
                         Builders<LeaderboardRowMongoDB>.Filter.Eq("ClientId", clientId)
            );

            var row = new LeaderboardRowMongoDB(new LeaderboardRowDTO());
            try 
            {
                row = await _rows.Find(filter).SingleAsync();
            }
            catch (Exception e)
            {
                Log.Information(e.Message);
                return null;
            }
            return row.ToLeaderboardRowDTO();
        }

        public async Task<List<LeaderboardRowDTO>> GetRowsPaginated(short leaderboardId, int offset, int limit)
        {
            // With help from Kevin Smith at https://kevsoft.net/2020/01/27/paging-data-in-mongodb-with-csharp.html          
            var filter = Builders<LeaderboardRowMongoDB>.Filter.Eq("LeaderboardId", leaderboardId);

            var dataFacet = AggregateFacet.Create("Data",
                PipelineDefinition<LeaderboardRowMongoDB, LeaderboardRowMongoDB>.Create(
                    new[]
                    {
                        PipelineStageDefinitionBuilder.Sort(Builders<LeaderboardRowMongoDB>.Sort.Descending(row => row.Rating)),
                        PipelineStageDefinitionBuilder.Skip<LeaderboardRowMongoDB>(offset),
                        PipelineStageDefinitionBuilder.Limit<LeaderboardRowMongoDB>(limit),
                    }));

            var aggregate = await _rows.Aggregate()
                .Match(filter)
                .Facet(dataFacet)
                .ToListAsync();

            var data = aggregate.First()
                .Facets.First(x => x.Name == "Data")
                .Output<LeaderboardRowMongoDB>();
            
            return data.Select(x => x.ToLeaderboardRowDTO()).ToList();
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
            var filter = Builders<LeaderboardRowMongoDB>.Filter.And(
                Builders<LeaderboardRowMongoDB>.Filter.Eq("LeaderboardId", rowIn.LeaderboardId),
                Builders<LeaderboardRowMongoDB>.Filter.Eq("ClientId", rowIn.ClientId)
            );
            await _rows.DeleteOneAsync(filter);
        }

        public async Task Delete(Guid clientId, short leaderboardId)
        {
            var filter = Builders<LeaderboardRowMongoDB>.Filter.And(
                Builders<LeaderboardRowMongoDB>.Filter.Eq("LeaderboardId", leaderboardId),
                Builders<LeaderboardRowMongoDB>.Filter.Eq("ClientId", clientId)
            );
            await _rows.DeleteOneAsync(filter);
        }

        public async Task DeleteAll()
        {
            await _rows.DeleteManyAsync(Builders<LeaderboardRowMongoDB>.Filter.Empty);
        }
    }
}