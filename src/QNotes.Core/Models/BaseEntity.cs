using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNotes.Core.Models
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
            Created = DateTimeOffset.UtcNow;
            Modified = null;
        }

        public Guid Id { get; set; }

        public DateTimeOffset Created { get; private set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
