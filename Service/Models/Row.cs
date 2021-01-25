using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LeaderboardAPI.Models
{
    public class Row
    {
        [BsonId]
        public ObjectId Id { get; set;}

        [Required]
        [BsonElement("ClientId")]
        [JsonProperty("ClientId")]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid client id supplied")]
        public long ClientId {get; set;}

        [Required]
        [BsonElement("Rating")]
        [JsonProperty("Rating")]
        [Range(1, long.MaxValue, ErrorMessage = "Invalid rating supplied")]
        public long Rating {get; set;}

    }
}
    
