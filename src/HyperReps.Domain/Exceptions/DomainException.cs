using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) {}
    }
}
