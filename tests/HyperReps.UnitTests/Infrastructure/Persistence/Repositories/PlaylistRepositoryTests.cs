using HyperReps.Domain.Entities;
using HyperReps.Domain.Enums;
using HyperReps.Infrastructure.Persistence;
using HyperReps.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HyperReps.UnitTests.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepositoryTests
    {
        private readonly DbContextOptions<HyperRepsContext> _options;
        private readonly IConfiguration _configuration;

        public PlaylistRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<HyperRepsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var myConfiguration = new Dictionary<string, string?>
            {
                {"Spotify:EncryptionKey", "TestKey1234567890123456789012345"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
        }

        [Fact]
        public async Task AddAsync_ShouldAddPlaylistToDatabase()
        {
            // Arrange
            using var context = new HyperRepsContext(_options, _configuration);
            var repository = new PlaylistRepository(context);
            var playlist = new Playlist(Guid.NewGuid(), Guid.NewGuid(), "spotify:playlist:123", "Test Playlist", "Desc", "url");

            // Act
            await repository.AddAsync(playlist);

            // Assert
            using var assertContext = new HyperRepsContext(_options, _configuration);
            var savedPlaylist = await assertContext.Playlists.FindAsync(playlist.Id);
            Assert.NotNull(savedPlaylist);
            Assert.Equal("Test Playlist", savedPlaylist.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPlaylist_WhenPlaylistExists()
        {
            // Arrange
            var playlistId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var playlist = new Playlist(playlistId, Guid.NewGuid(), "spotify:playlist:existing", "Existing Playlist", "Desc", "url");
                context.Playlists.Add(playlist);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new PlaylistRepository(context);
                var result = await repository.GetByIdAsync(playlistId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(playlistId, result.Id);
                Assert.Equal("Existing Playlist", result.Name);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePlaylistInDatabase()
        {
            // Arrange
            var playlistId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var playlist = new Playlist(playlistId, Guid.NewGuid(), "spotify:playlist:update", "Original Title", "Desc", "url");
                context.Playlists.Add(playlist);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new PlaylistRepository(context);
                var playlistToUpdate = await repository.GetByIdAsync(playlistId);
                playlistToUpdate!.UpdateDetails("Updated Title", "New Desc", "New url");
                await repository.UpdateAsync(playlistToUpdate);
            }

            // Assert
            using (var assertContext = new HyperRepsContext(_options, _configuration))
            {
                var updatedPlaylist = await assertContext.Playlists.FindAsync(playlistId);
                Assert.NotNull(updatedPlaylist);
                Assert.Equal("Updated Title", updatedPlaylist.Name);
                Assert.Equal("New Desc", updatedPlaylist.Description);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePlaylistFromDatabase()
        {
            // Arrange
            var playlistId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var playlist = new Playlist(playlistId, Guid.NewGuid(), "spotify:playlist:delete", "Playlist To Delete", "Desc", "url");
                context.Playlists.Add(playlist);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new PlaylistRepository(context);
                var playlistToDelete = await repository.GetByIdAsync(playlistId);
                await repository.DeleteAsync(playlistToDelete!);
            }

            // Assert
            using (var assertContext = new HyperRepsContext(_options, _configuration))
            {
                var deletedPlaylist = await assertContext.Playlists.FindAsync(playlistId);
                Assert.Null(deletedPlaylist);
            }
        }
    }
}
