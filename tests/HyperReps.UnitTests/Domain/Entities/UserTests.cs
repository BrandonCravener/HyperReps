using HyperReps.Domain.Entities;
using HyperReps.Domain.Exceptions;
using HyperReps.Domain.ValueObjects;

namespace HyperReps.UnitTests.Domain.Entities
{
    public class UserTests
    {
        private readonly SpotifyCredentials _validCredentials;

        public UserTests()
        {
            _validCredentials = new SpotifyCredentials("access", "refresh", DateTimeOffset.UtcNow.AddHours(1));
        }

        [Fact]
        public void Constructor_ShouldCreateUser_WhenArgumentsAreValid()
        {

            var id = Guid.NewGuid();
            var spotifyId = "spotify_123";
            var email = "test@example.com";
            var name = "Test User";
            var avatar = "http://avatar.url";


            var user = new User(id, spotifyId, email, name, avatar, _validCredentials);


            Assert.Equal(id, user.Id);
            Assert.Equal(spotifyId, user.SpotifyId);
            Assert.Equal(email, user.Email);
            Assert.Equal(name, user.DisplayName);
            Assert.Equal(avatar, user.AvatarUrl);
            Assert.Equal(_validCredentials, user.Credentials);
            Assert.Empty(user.Mixes);
            Assert.Empty(user.Playlists);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ShouldThrowDateValidationException_WhenSpotifyIdIsInvalid(string? invalidSpotifyId)
        {

            Assert.Throws<UserValidationException>(() =>
                new User(Guid.NewGuid(), invalidSpotifyId!, "email", "name", "avatar", _validCredentials));
        }

        [Fact]
        public void Constructor_ShouldThrowValidationException_WhenCredentialsAreNull()
        {

            Assert.Throws<UserValidationException>(() =>
                new User(Guid.NewGuid(), "spotify_123", "email", "name", "avatar", null!));
        }

        [Fact]
        public void UpdateProfile_ShouldUpdateFields_WhenArgumentsAreValid()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "Old Name", "old_pfp", _validCredentials);
            var newName = "New Name";
            var newAvatar = "new_pfp";


            user.UpdateProfile(newName, newAvatar);


            Assert.Equal(newName, user.DisplayName);
            Assert.Equal(newAvatar, user.AvatarUrl);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateProfile_ShouldThrowValidationException_WhenDisplayNameIsInvalid(string? invalidName)
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "Old Name", "old_pfp", _validCredentials);


            Assert.Throws<UserValidationException>(() => user.UpdateProfile(invalidName!, "avatar"));
        }

        [Fact]
        public void UpdateCredentials_ShouldUpdateCredentials_WhenValid()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "name", "avatar", _validCredentials);
            var newCreds = new SpotifyCredentials("new_access", "new_refresh", DateTimeOffset.UtcNow);


            user.UpdateCredentials(newCreds);


            Assert.Equal(newCreds, user.Credentials);
        }

        [Fact]
        public void UpdateCredentials_ShouldThrowValidationException_WhenNull()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "name", "avatar", _validCredentials);


            Assert.Throws<UserValidationException>(() => user.UpdateCredentials(null!));
        }

        [Fact]
        public void AddMix_ShouldAddMixToCollection_WhenValid()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "name", "avatar", _validCredentials);
            var mix = new Mix(Guid.NewGuid(), user.Id, "My Mix", "Desc", "Thumb", true);


            user.AddMix(mix);


            Assert.Single(user.Mixes);
            Assert.Contains(mix, user.Mixes);
        }

        [Fact]
        public void AddMix_ShouldThrowException_WhenMixIsNull()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "name", "avatar", _validCredentials);


            Assert.Throws<UserValidationException>(() => user.AddMix(null!));
        }

        [Fact]
        public void AddMix_ShouldThrowException_WhenMixAlreadyExists()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "name", "avatar", _validCredentials);
            var mix = new Mix(Guid.NewGuid(), user.Id, "My Mix", "Desc", "Thumb", true);
            user.AddMix(mix);


            Assert.Throws<UserValidationException>(() => user.AddMix(mix));
        }

        [Fact]
        public void AddPlaylist_ShouldAddPlaylistToCollection_WhenValid()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "name", "avatar", _validCredentials);
            var playlist = new Playlist(Guid.NewGuid(), user.Id, "pid", "Name", "Desc", "Thumb");


            user.AddPlaylist(playlist);


            Assert.Single(user.Playlists);
            Assert.Contains(playlist, user.Playlists);
        }

        [Fact]
        public void AddPlaylist_ShouldThrowException_WhenPlaylistAlreadyExists()
        {

            var user = new User(Guid.NewGuid(), "sid", "email", "name", "avatar", _validCredentials);
            var playlist = new Playlist(Guid.NewGuid(), user.Id, "pid", "Name", "Desc", "Thumb");
            user.AddPlaylist(playlist);


            Assert.Throws<UserValidationException>(() => user.AddPlaylist(playlist));
        }
    }
}
