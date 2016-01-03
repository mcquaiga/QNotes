using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QNotes.API.Models
{
    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
        DateTimeOffset Created { get; }
        DateTimeOffset? Modified { get; set; }
    }

    public abstract class MongoEntity : IMongoEntity
    {
        public MongoEntity()
        {
            Created = DateTimeOffset.UtcNow;
            Modified = null;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public DateTimeOffset Created { get; }
        public DateTimeOffset? Modified { get; set; }
    }
}
