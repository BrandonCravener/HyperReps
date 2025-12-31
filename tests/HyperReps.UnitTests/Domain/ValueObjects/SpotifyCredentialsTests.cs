using HyperReps.Domain.ValueObjects;
using HyperReps.Domain.Exceptions;

namespace HyperReps.UnitTests.Domain.ValueObjects
{
    public class SpotifyCredentialsTests
    {
        [Fact]
        public void IsExpired_ShouldReturnTrue_WhenExpiryIsInPast()
        {
            var past = DateTimeOffset.UtcNow.AddMinutes(-10);
            var creds = new SpotifyCredentials("access", "refresh", past);

            Assert.True(creds.IsExpired());
        }

        [Fact]
        public void IsExpired_ShouldReturnFalse_WhenExpiryIsInFuture()
        {
            // Arrange
            var future = DateTimeOffset.UtcNow.AddMinutes(20);
            var creds = new SpotifyCredentials("access", "refresh", future);

            // Act
            Assert.False(creds.IsExpired());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ShouldThrowException_WhenAccessTokenInvalid(string? invalidToken)
        {
            Assert.Throws<UserValidationException>(() =>
                new SpotifyCredentials(invalidToken!, "refresh", DateTimeOffset.UtcNow));
        }
    }
}
