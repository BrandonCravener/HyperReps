using HyperReps.Domain.Exceptions;

namespace HyperReps.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; } = Guid.CreateVersion7();


        protected BaseEntity(Guid id)
        {
            if (id == Guid.Empty) throw EntityValidationException.InvalidGuid();

            Id = id;
        }
    }
}
