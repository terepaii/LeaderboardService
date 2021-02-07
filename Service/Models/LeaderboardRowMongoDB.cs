using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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
        public long ClientId {get; set;}

        [Required]
        [BsonElement("Rating")]
        [JsonProperty("Rating")]
        public long Rating {get; set;}

        public LeaderboardRowMongoDB(LeaderboardRowDTO rowIn)
        {
            ClientId = rowIn.ClientId;
            Rating = rowIn.Rating;
        }
    }
}
    
