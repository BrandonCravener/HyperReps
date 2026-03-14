namespace HyperReps.Domain.Exceptions
{
    public class PlaylistTrackValidationException : DomainException
    {
        public PlaylistTrackValidationException(string message) : base(message) {}

        public static PlaylistTrackValidationException InvalidPlaylistId() => new("The provided Playlist ID is invalid.");
        public static PlaylistTrackValidationException InvalidTrackId() => new("The provided Track ID is invalid.");
    }
}
