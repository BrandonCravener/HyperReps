using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyperReps.Domain.Exceptions;
using HyperReps.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using Xunit;

namespace HyperReps.UnitTests.Infrastructure.Services;

public class SpotifyAuthServiceTests
{
    [Fact]
    public async Task RefreshTokenAsync_WithFakeTokenAndId_ThrowsException()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Spotify:ClientId", "fake_client_id"},
            {"Spotify:ClientSecret", "fake_secret"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var service = new SpotifyAuthService(configuration);

        // Act & Assert
        // Using fake credentials will cause Spotify's API to reject the request and throw an exception.
        // It might be RefreshTokenRevokedException if "invalid_grant" is the response, or a broader APIException
        await Assert.ThrowsAnyAsync<Exception>(() => service.RefreshTokenAsync("fake_refresh_token"));
    }
}
