using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System; 
using LeaderboardAPI.Interfaces;

namespace LeaderboardAPI.Models
{
    public class LeaderboardRowMongoDB
    {
        [BsonId]
        private ObjectId _id { get; set;}

        [Required]
        [BsonElement("ClientId")]
        [JsonProperty("ClientId")]
        public Guid ClientId {get; set;}

        [Required]
        [BsonElement("Rating")]
        [JsonProperty("Rating")]
        public long Rating {get; set;}

        [Required]
        [BsonElement("LeaderboardId")]
        [JsonProperty("LeaderboardId")]
        public short LeaderboardId {get; set;}

        public LeaderboardRowMongoDB(LeaderboardRowDTO rowIn)
        {
            ClientId = rowIn.ClientId;
            Rating = rowIn.Rating;
            LeaderboardId = rowIn.LeaderboardId;
        }

        public LeaderboardRowDTO ToLeaderboardRowDTO()
        {
            return new LeaderboardRowDTO {
                ClientId = this.ClientId,
                Rating = this.Rating,
                LeaderboardId = this.LeaderboardId
            };
        }
    }
}
    
