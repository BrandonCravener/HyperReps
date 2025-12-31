using HyperReps.Domain.Entities;
using HyperReps.Domain.Exceptions;

namespace HyperReps.UnitTests.Domain.Entities
{
    public class TrackTests
    {
        [Fact]
        public void Constructor_ShouldCreateTrack_WhenArgumentsAreValid()
        {

            var id = Guid.NewGuid();
            var spotifyId = "track_123";
            var title = "Song";
            var artist = "Artist";
            var album = "Album";
            var art = "http://art.url";
            var duration = 1000;
            var preview = "http://preview.url";


            var track = new Track(id, spotifyId, title, artist, album, art, duration, preview);


            Assert.Equal(id, track.Id);
            Assert.Equal(spotifyId, track.SpotifyTrackId);
            Assert.Equal(title, track.Title);
            Assert.Equal(artist, track.ArtistName);
            Assert.Equal(album, track.AlbumName);
            Assert.Equal(duration, track.DurationMs);
            Assert.Equal(preview, track.PreviewUrl);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void Constructor_ShouldThrowException_WhenSpotifyIdIsInvalid(string? invalidId)
        {
            Assert.Throws<TrackValidationException>(() =>
               new Track(Guid.NewGuid(), invalidId!, "Title", "Artist", "Album", "Art", 100, "Preview"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_ShouldThrowException_WhenDurationIsInvalid(int invalidDuration)
        {
            Assert.Throws<TrackValidationException>(() =>
               new Track(Guid.NewGuid(), "id", "Title", "Artist", "Album", "Art", invalidDuration, "Preview"));
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateFields_WhenValid()
        {

            var track = new Track(Guid.NewGuid(), "id", "Title", "Artist", "Album", "Art", 100, "Preview");
            var newTitle = "New Title";
            var newArtist = "New Artist";
            var newAlbum = "New Album";
            var newArt = "New Art";
            var newDuration = 200;
            var newPreview = "New Preview";


            track.UpdateDetails(newTitle, newArtist, newAlbum, newArt, newDuration, newPreview);


            Assert.Equal(newTitle, track.Title);
            Assert.Equal(newArtist, track.ArtistName);
            Assert.Equal(newAlbum, track.AlbumName);
            Assert.Equal(newArt, track.AlbumArtUrl);
            Assert.Equal(newDuration, track.DurationMs);
            Assert.Equal(newPreview, track.PreviewUrl);
        }

        [Fact]
        public void UpdateDetails_ShouldThrowException_WhenValidationFails()
        {

            var track = new Track(Guid.NewGuid(), "id", "Title", "Artist", "Album", "Art", 100, "Preview");


            Assert.Throws<TrackValidationException>(() =>
                track.UpdateDetails("", "Artist", "Album", "Art", 100, "Preview"));
        }
    }
}
