using System;
using System.Collections.Generic;
using System.Text;

namespace HyperReps.Domain.Exceptions
{
    public class TrackValidationException : DomainException
    {
        public TrackValidationException(string message) : base(message) {}

        public static TrackValidationException InvalidSpotifyTrackId() => new("Spotify Track ID cannot be null or empty.");
        public static TrackValidationException InvalidTitle() => new("Track title cannot be null or empty.");
        public static TrackValidationException InvalidArtist() => new("Track artist cannot be null or empty.");
        public static TrackValidationException InvalidAlbum() => new("Track album cannot be null or empty.");
        public static TrackValidationException InvalidDuration() => new("Track duration must be a positive value.");
    }
}
