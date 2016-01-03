using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QNotes.API.Models
{
    public class Note : MongoEntity
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
