using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNotes.API.Models
{
    public class Tag : MongoEntity
    {
        public string Name { get; set; }
    }
}
