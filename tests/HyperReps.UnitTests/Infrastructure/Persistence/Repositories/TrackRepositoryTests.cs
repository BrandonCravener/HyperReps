using HyperReps.Domain.Entities;
using HyperReps.Infrastructure.Persistence;
using HyperReps.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HyperReps.UnitTests.Infrastructure.Persistence.Repositories
{
    public class TrackRepositoryTests
    {
        private readonly DbContextOptions<HyperRepsContext> _options;
        private readonly IConfiguration _configuration;

        public TrackRepositoryTests()
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
        public async Task AddAsync_ShouldAddTrackToDatabase()
        {
            // Arrange
            using var context = new HyperRepsContext(_options, _configuration);
            var repository = new TrackRepository(context);
            var track = new Track(Guid.NewGuid(), "spotify:track:123", "Test Track", "Artist", "Album", "url", 200, "preview");

            // Act
            await repository.AddAsync(track);

            // Assert
            using var assertContext = new HyperRepsContext(_options, _configuration);
            var savedTrack = await assertContext.Tracks.FindAsync(track.Id);
            Assert.NotNull(savedTrack);
            Assert.Equal("Test Track", savedTrack.Title);
        }

        [Fact]
        public async Task GetBySpotifyIdAsync_ShouldReturnTrack_WhenTrackExists()
        {
            // Arrange
            var spotifyId = "spotify:track:existing";
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var track = new Track(Guid.NewGuid(), spotifyId, "Existing Track", "Artist", "Album", "url", 200, "preview");
                context.Tracks.Add(track);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new TrackRepository(context);
                var result = await repository.GetBySpotifyIdAsync(spotifyId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(spotifyId, result.SpotifyTrackId);
            }
        }

        [Fact]
        public async Task GetBySpotifyIdAsync_ShouldReturnNull_WhenTrackDoesNotExist()
        {
            // Arrange
            using var context = new HyperRepsContext(_options, _configuration);
            var repository = new TrackRepository(context);

            // Act
            var result = await repository.GetBySpotifyIdAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }
    }
}
