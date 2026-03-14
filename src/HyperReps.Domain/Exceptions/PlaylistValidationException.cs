using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.Exceptions
{
    public class PlaylistValidationException : DomainException
    {
        public PlaylistValidationException(string message) : base(message) { }

        public static PlaylistValidationException InvalidName() => new("Playlist name cannot be null or empty.");
        public static PlaylistValidationException InvalidUserId() => new("User ID must be a valid non-empty GUID.");
        public static PlaylistValidationException InvalidSpotifyPlaylistId() => new("Spotify Playlist ID cannot be null or empty.");
        public static PlaylistValidationException TrackCannotBeNull() => new("Track cannot be null.");
    }
}
