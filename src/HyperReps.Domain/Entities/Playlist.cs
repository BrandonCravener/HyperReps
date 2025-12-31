using HyperReps.Domain.Common;
using HyperReps.Domain.Enums;
using HyperReps.Domain.Exceptions;

namespace HyperReps.Domain.Entities
{
    public class Playlist : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public string SpotifyPlaylistId { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public bool IsSyncActive { get; private set; }
        public SyncStatus SyncStatus { get; private set; }
        public string? LastErrorMessage { get; private set; }
        public DateTimeOffset? LastSyncedAt { get; private set; }

        public User? User { get; private set; }

        private readonly List<PlaylistTrack> _playlistTracks = new();
        public IReadOnlyCollection<PlaylistTrack> PlaylistTracks => _playlistTracks.AsReadOnly();

        private Playlist() : base() {}

        public Playlist(Guid id, Guid userId, string spotifyPlaylistId, string name, string? description, string? thumbnailUrl)
            : base(id)
        {
            Validate(userId, spotifyPlaylistId, name);

            UserId = userId;
            SpotifyPlaylistId = spotifyPlaylistId;
            Name = name;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            IsSyncActive = true;
            SyncStatus = SyncStatus.Idle;
        }

        public void UpdateDetails(string name, string? description, string? thumbnailUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw PlaylistValidationException.InvalidName();

            Name = name;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
        }

        public void UpdateSyncStatus(SyncStatus status, string? errorMessage = null)
        {
            SyncStatus = status;
            LastErrorMessage = errorMessage;

            if (status == SyncStatus.Idle)
            {
                LastSyncedAt = DateTimeOffset.UtcNow;
            }
        }

        public void SetSyncActive(bool isActive)
        {
            IsSyncActive = isActive;
        }

        public void AddTrack(Track track, DateTimeOffset addedAtTimestamp)
        {
            if (track == null) throw PlaylistValidationException.TrackCannotBeNull();

            if (!_playlistTracks.Any(pt => pt.TrackId == track.Id))
            {
                _playlistTracks.Add(new PlaylistTrack(Id, track.Id, addedAtTimestamp));
            }
        }

        public void RemoveTrack(Guid trackId)
        {
            var track = _playlistTracks.FirstOrDefault(pt => pt.TrackId == trackId);
            if (track != null)
            {
                _playlistTracks.Remove(track);
            }
        }

        private void Validate(Guid userId, string spotifyPlaylistId, string name)
        {
            if (userId == Guid.Empty) throw PlaylistValidationException.InvalidUserId();
            if (string.IsNullOrWhiteSpace(spotifyPlaylistId)) throw PlaylistValidationException.InvalidSpotifyPlaylistId();
            if (string.IsNullOrWhiteSpace(name)) throw PlaylistValidationException.InvalidName();
        }
    }
}