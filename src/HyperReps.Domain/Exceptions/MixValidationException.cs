namespace HyperReps.Domain.Exceptions
{
    public class MixValidationException : DomainException
    {
        public MixValidationException(string message) : base(message) { }

        public static MixValidationException InvalidName() => new("Mix name cannot be empty.");
        public static MixValidationException InvalidUserId() => new("A valid User ID is required.");
        public static MixValidationException NegativeDuration() => new("Total duration cannot be negative.");
    }
}
