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
    public class MixRepositoryTests
    {
        private readonly DbContextOptions<HyperRepsContext> _options;
        private readonly IConfiguration _configuration;

        public MixRepositoryTests()
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
        public async Task AddAsync_ShouldAddMixToDatabase()
        {
            // Arrange
            using var context = new HyperRepsContext(_options, _configuration);
            var repository = new MixRepository(context);
            var mix = new Mix(Guid.NewGuid(), Guid.NewGuid(), "Test Mix", "Desc", "url", true);

            // Act
            await repository.AddAsync(mix);

            // Assert
            using var assertContext = new HyperRepsContext(_options, _configuration);
            var savedMix = await assertContext.Mixes.FindAsync(mix.Id);
            Assert.NotNull(savedMix);
            Assert.Equal("Test Mix", savedMix.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMix_WhenMixExists()
        {
            // Arrange
            var mixId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var mix = new Mix(mixId, Guid.NewGuid(), "Existing Mix", "Desc", "url", true);
                context.Mixes.Add(mix);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new MixRepository(context);
                var result = await repository.GetByIdAsync(mixId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(mixId, result.Id);
                Assert.Equal("Existing Mix", result.Name);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenMixDoesNotExist()
        {
            // Arrange
            using var context = new HyperRepsContext(_options, _configuration);
            var repository = new MixRepository(context);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateMixInDatabase()
        {
            // Arrange
            var mixId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var mix = new Mix(mixId, Guid.NewGuid(), "Original Title", "Desc", "url", true);
                context.Mixes.Add(mix);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new MixRepository(context);
                var mixToUpdate = await repository.GetByIdAsync(mixId);
                mixToUpdate!.UpdateDetails("Updated Title", "New Desc", "New url", false);
                await repository.UpdateAsync(mixToUpdate);
            }

            // Assert
            using (var assertContext = new HyperRepsContext(_options, _configuration))
            {
                var updatedMix = await assertContext.Mixes.FindAsync(mixId);
                Assert.NotNull(updatedMix);
                Assert.Equal("Updated Title", updatedMix.Name);
                Assert.Equal("New Desc", updatedMix.Description);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMixFromDatabase()
        {
            // Arrange
            var mixId = Guid.NewGuid();
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var mix = new Mix(mixId, Guid.NewGuid(), "Mix To Delete", "Desc", "url", true);
                context.Mixes.Add(mix);
                await context.SaveChangesAsync();
            }
            
            // Act
            using (var context = new HyperRepsContext(_options, _configuration))
            {
                var repository = new MixRepository(context);
                var mixToDelete = await repository.GetByIdAsync(mixId);
                await repository.DeleteAsync(mixToDelete!);
            }

            // Assert
            using (var assertContext = new HyperRepsContext(_options, _configuration))
            {
                var deletedMix = await assertContext.Mixes.FindAsync(mixId);
                Assert.Null(deletedMix);
            }
        }
    }
}
