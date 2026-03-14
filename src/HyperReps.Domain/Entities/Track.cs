using HyperReps.Domain.Common;
using HyperReps.Domain.Exceptions;

namespace HyperReps.Domain.Entities
{
    public class Track : AuditableEntity
    {

        public string SpotifyTrackId { get; private set; } = null!;
        public string Title { get; private set; } = null!;
        public string ArtistName { get; private set; } = null!;
        public string AlbumName { get; private set; } = null!;
        public string AlbumArtUrl { get; private set; } = null!;
        public int DurationMs { get; private set; }
        public string? PreviewUrl { get; private set; }

        private readonly List<MixSegment> _referencedBySegments = new();
        public IReadOnlyCollection<MixSegment> ReferencedBySegments => _referencedBySegments.AsReadOnly();

        private readonly List<PlaylistTrack> _includedInPlaylists = new();
        public IReadOnlyCollection<PlaylistTrack> IncludedInPlaylists => _includedInPlaylists.AsReadOnly();

        private Track(): base() { }

        public Track(Guid id, string spotifyTrackId, string title, string artistName, string albumName,
                     string albumArtUrl, int durationMs, string? previewUrl) : base(id)
        {
            Validate(spotifyTrackId, title, artistName, albumName, durationMs);

            SpotifyTrackId = spotifyTrackId;
            Title = title;
            ArtistName = artistName;
            AlbumName = albumName;
            AlbumArtUrl = albumArtUrl;
            DurationMs = durationMs;
            PreviewUrl = previewUrl;
        }

        public void UpdateDetails(string title, string artistName, string albumName,
                                  string albumArtUrl, int durationMs, string? previewUrl)
        {

             Validate(SpotifyTrackId, title, artistName, albumName, durationMs);

            Title = title;
            ArtistName = artistName;
            AlbumName = albumName;
            AlbumArtUrl = albumArtUrl;
            DurationMs = durationMs;
            PreviewUrl = previewUrl;
        }

        private void Validate(string spotifyTrackId, string title, string artistName, string albumName, int durationMs)
        {
            if (string.IsNullOrWhiteSpace(spotifyTrackId)) throw TrackValidationException.InvalidSpotifyTrackId();
            if (string.IsNullOrWhiteSpace(title)) throw TrackValidationException.InvalidTitle();
            if (string.IsNullOrWhiteSpace(artistName)) throw TrackValidationException.InvalidArtist();
            if (string.IsNullOrWhiteSpace(albumName)) throw TrackValidationException.InvalidAlbum();
            if (durationMs <= 0) throw TrackValidationException.InvalidDuration();
        }
    }
}
