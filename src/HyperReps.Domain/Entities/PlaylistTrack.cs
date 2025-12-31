using HyperReps.Domain.Exceptions;

namespace HyperReps.Domain.Entities
{
    public class PlaylistTrack
    {
        public Guid PlaylistId { get; private set; }
        public Guid TrackId { get; private set; }
        public DateTimeOffset AddedAt { get; private set; }

        public  Playlist? Playlist { get; private set; }
        public Track? Track { get; private set; }

        private PlaylistTrack() {}

        public PlaylistTrack(Guid playlistId, Guid trackId, DateTimeOffset addedAtTimestamp)
        {
            if (playlistId == Guid.Empty) throw PlaylistTrackValidationException.InvalidPlaylistId();
            if (trackId == Guid.Empty) throw PlaylistTrackValidationException.InvalidTrackId();

            PlaylistId = playlistId;
            TrackId = trackId;
            AddedAt = addedAtTimestamp;
        }
    }
}
