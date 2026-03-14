namespace HyperReps.Domain.Exceptions
{
    public class RefreshTokenRevokedException : DomainException
    {
        public RefreshTokenRevokedException(
            string message = "The users Spotify resfresh token has expired or been revoked."
        )
            : base(message) { }
    }
}
