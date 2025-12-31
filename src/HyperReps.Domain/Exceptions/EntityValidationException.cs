using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.Exceptions
{
    public class EntityValidationException : DomainException
    {
        public EntityValidationException(string message) : base(message) {}

        public static EntityValidationException InvalidGuid() => new("The provided GUID is invalid.");
    }
}
