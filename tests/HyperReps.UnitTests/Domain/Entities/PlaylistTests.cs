using HyperReps.Domain.Entities;
using HyperReps.Domain.Enums;
using HyperReps.Domain.Exceptions;

namespace HyperReps.UnitTests.Domain.Entities
{
    public class PlaylistTests
    {
        [Fact]
        public void Constructor_ShouldCreatePlaylist_WhenArgumentsAreValid()
        {

            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var spotifyId = "pid";
            var name = "My Playlist";
            var desc = "Description";
            var thumb = "Thumb";


            var playlist = new Playlist(id, userId, spotifyId, name, desc, thumb);


            Assert.Equal(id, playlist.Id);
            Assert.Equal(userId, playlist.UserId);
            Assert.Equal(spotifyId, playlist.SpotifyPlaylistId);
            Assert.Equal(name, playlist.Name);
            Assert.True(playlist.IsSyncActive);
            Assert.Equal(SyncStatus.Idle, playlist.SyncStatus);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenUserIdIsEmpty()
        {
            Assert.Throws<PlaylistValidationException>(() =>
                new Playlist(Guid.NewGuid(), Guid.Empty, "pid", "Name", "Desc", "Thumb"));
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateFields_WhenValid()
        {

            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "pid", "Name", "Desc", "Thumb");
            var newName = "New Name";


            playlist.UpdateDetails(newName, "New Desc", "New Thumb");


            Assert.Equal(newName, playlist.Name);
            Assert.Equal("New Desc", playlist.Description);
            Assert.Equal("New Thumb", playlist.ThumbnailUrl);
        }

        [Fact]
        public void UpdateSyncStatus_ShouldUpdateStatusAndTimestamp_WhenIdle()
        {

            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "pid", "Name", "Desc", "Thumb");
            var initialSync = playlist.LastSyncedAt;


            playlist.UpdateSyncStatus(SyncStatus.Syncing);
            Assert.Equal(SyncStatus.Syncing, playlist.SyncStatus);
            Assert.Equal(initialSync, playlist.LastSyncedAt);

            playlist.UpdateSyncStatus(SyncStatus.Idle);


            Assert.Equal(SyncStatus.Idle, playlist.SyncStatus);
            Assert.NotNull(playlist.LastSyncedAt);
            if (initialSync.HasValue)
            {
                Assert.True(playlist.LastSyncedAt > initialSync);
            }
        }

        [Fact]
        public void UpdateSyncStatus_ShouldCaptureErrorMessage_WhenFailed()
        {

            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "pid", "Name", "Desc", "Thumb");
            var error = "Something went wrong";


            playlist.UpdateSyncStatus(SyncStatus.Failed, error);


            Assert.Equal(SyncStatus.Failed, playlist.SyncStatus);
            Assert.Equal(error, playlist.LastErrorMessage);
        }

        [Fact]
        public void SetSyncActive_ShouldUpdateFlag()
        {

            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "pid", "Name", "Desc", "Thumb");


            playlist.SetSyncActive(false);


            Assert.False(playlist.IsSyncActive);
        }

        [Fact]
        public void AddTrack_ShouldAddTrack_WhenNotExists()
        {

            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "pid", "Name", "Desc", "Thumb");
            var track = new Track(Guid.NewGuid(), "sid", "Title", "Artist", "Album", "Art", 100, "Preview");
            var addedAt = DateTimeOffset.UtcNow;


            playlist.AddTrack(track, addedAt);


            Assert.Single(playlist.PlaylistTracks);
            var pt = playlist.PlaylistTracks.First();
            Assert.Equal(track.Id, pt.TrackId);
            Assert.Equal(playlist.Id, pt.PlaylistId);
            Assert.Equal(addedAt, pt.AddedAt);
        }

        [Fact]
        public void AddTrack_ShouldNotAddDuplicate()
        {

            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "pid", "Name", "Desc", "Thumb");
            var track = new Track(Guid.NewGuid(), "sid", "Title", "Artist", "Album", "Art", 100, "Preview");
            playlist.AddTrack(track, DateTimeOffset.UtcNow);


            playlist.AddTrack(track, DateTimeOffset.UtcNow);


            Assert.Single(playlist.PlaylistTracks);
        }

        [Fact]
        public void RemoveTrack_ShouldRemoveTrack_WhenExists()
        {

            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "pid", "Name", "Desc", "Thumb");
            var track = new Track(Guid.NewGuid(), "sid", "Title", "Artist", "Album", "Art", 100, "Preview");
            playlist.AddTrack(track, DateTimeOffset.UtcNow);


            playlist.RemoveTrack(track.Id);


            Assert.Empty(playlist.PlaylistTracks);
        }
    }
}
