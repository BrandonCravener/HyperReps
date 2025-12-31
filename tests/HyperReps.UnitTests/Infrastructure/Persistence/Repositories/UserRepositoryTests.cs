using HyperReps.Domain.Entities;
using HyperReps.Domain.ValueObjects;
using HyperReps.Infrastructure.Persistence;
using HyperReps.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HyperReps.UnitTests.Infrastructure.Persistence.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<HyperRepsContext> _options;
        private readonly IConfiguration _configuration;

        public UserRepositoryTests()
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

        private User CreateTestUser(Guid id, string spotifyId)
        {
            var credentials = new SpotifyCredentials("access_token", "refresh_token", DateTimeOffset.UtcNow.AddHours(1));
            return new User(id, spotifyId, "test@example.com", "Test User", "avatar_url", credentials);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            using var context = new HyperRepsContext(_options, _configuration);
            var repository = new UserRepository(context);
            var user = CreateTestUser(Guid.NewGuid(), "spotify:user:123");

            // Act
            await repository.AddAsync(user);

            // Assert
            using var assertContext = new HyperRepsContext(_options, _configuration);
            var savedUser = await assertContext.Users.FindAsync(user.Id);
            Assert.NotNull(savedUser);
            Assert.Equal("spotify:user:123", savedUser.SpotifyId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var user = CreateTestUser(userId, "spotify:user:existing");
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetByIdAsync(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userId, result.Id);
                Assert.Equal("spotify:user:existing", result.SpotifyId);
            }
        }

        [Fact]
        public async Task GetBySpotifyIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var spotifyId = "spotify:user:search";
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var user = CreateTestUser(Guid.NewGuid(), spotifyId);
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetBySpotifyIdAsync(spotifyId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(spotifyId, result.SpotifyId);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserInDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var user = CreateTestUser(userId, "spotify:user:update");
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new UserRepository(context);
                var userToUpdate = await repository.GetByIdAsync(userId);
                userToUpdate!.UpdateProfile("Updated Name", "new_avatar_url");
                await repository.UpdateAsync(userToUpdate);
            }

            // Assert
            using (var assertContext = new HyperRepsContext(_options, _configuration))
            {
                var updatedUser = await assertContext.Users.FindAsync(userId);
                Assert.NotNull(updatedUser);
                Assert.Equal("Updated Name", updatedUser.DisplayName);
                Assert.Equal("new_avatar_url", updatedUser.AvatarUrl);
            }
        }
    }
}
