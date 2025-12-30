using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public AuditableEntity(Guid id) : base(id)
        {
        }

        public DateTimeOffset CreatedAt { get; internal set; }
        public DateTimeOffset? ModifiedAt { get; internal set; }
    }
}
