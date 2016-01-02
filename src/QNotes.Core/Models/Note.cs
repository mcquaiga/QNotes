using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QNotes.Core.Models
{
    public class Note : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
