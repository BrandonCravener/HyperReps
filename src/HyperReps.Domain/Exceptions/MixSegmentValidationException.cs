namespace HyperReps.Domain.Exceptions
{
    public class MixSegmentValidationException : DomainException
    {
        public MixSegmentValidationException(string message) : base(message) { }

        public static MixSegmentValidationException InvalidTimeRange() => new("End time must be greater than start time.");
        public static MixSegmentValidationException InvalidSequence() => new("Sequence order must be zero or greater.");
    }
}
