namespace HyperReps.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule or invariant is violated within the User entity.
    /// </summary>
    public class UserValidationException : DomainException
    {
        public UserValidationException(string message) : base(message) {}

        public static UserValidationException InvalidSpotifyId() => new("Spotify ID cannot be null or empty.");
        public static UserValidationException InvalidDisplayName() => new ("Display name cannot be null or empty.");
        public static UserValidationException InvalidAccessToken() => new("Access token cannot be null or empty.");
        public static UserValidationException InvalidRefreshToken() => new("Refresh token cannot be null or empty.");
        public static UserValidationException CredentialsRequired() => new("Credentials are required.");
        public static UserValidationException CredentialsMustBeDefined() => new("Credentials must be defined.");
    }
}
