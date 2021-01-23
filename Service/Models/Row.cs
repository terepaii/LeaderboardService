using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace LeaderboardApi.Models
{
    public class Row
    {
        [BsonId]
        public ObjectId Id { get; set;}

        [BsonElement("ClientId")]
        [JsonProperty("ClientId")]
        public long ClientId {get; set;}

        [BsonElement("Rating")]
        [JsonProperty("Rating")]
        public long Rating {get; set;}

    }
}
    
