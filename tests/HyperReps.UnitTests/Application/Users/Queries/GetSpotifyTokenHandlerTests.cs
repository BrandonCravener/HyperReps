using System;
using System.Threading.Tasks;
using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Application.Common.Interfaces.Services;
using HyperReps.Application.Users.Queries;
using HyperReps.Domain.Entities;
using HyperReps.Domain.Exceptions;
using HyperReps.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace HyperReps.UnitTests.Application.Users.Queries;

public class GetSpotifyTokenHandlerTests
{
    private readonly IUserRepository _repositoryMock;
    private readonly ISpotifyAuthService _spotifyAuthServiceMock;
    private readonly GetSpotifyTokenHandler _handler;

    public GetSpotifyTokenHandlerTests()
    {
        _repositoryMock = Substitute.For<IUserRepository>();
        _spotifyAuthServiceMock = Substitute.For<ISpotifyAuthService>();
        _handler = new GetSpotifyTokenHandler();
    }

    [Fact]
    public async Task Handle_WithNoUser_ReturnsNull()
    {
        // Arrange
        var query = new GetSpotifyTokenQuery(Guid.NewGuid());
        _repositoryMock.GetByIdAsync(query.UserId).Returns((User?)null);

        // Act
        var result = await _handler.Handle(query, _repositoryMock, _spotifyAuthServiceMock);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithNoCredentials_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(userId, "spotify123", "email", "name", "avatar", new SpotifyCredentials("a", "r", DateTimeOffset.UtcNow));
        user.RevokeCredentials();
        
        var query = new GetSpotifyTokenQuery(userId);
        _repositoryMock.GetByIdAsync(query.UserId).Returns(user);

        // Act
        var result = await _handler.Handle(query, _repositoryMock, _spotifyAuthServiceMock);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expiry = DateTimeOffset.UtcNow.AddHours(1);
        var credentials = new SpotifyCredentials("access", "refresh", expiry);
        var user = new User(userId, "spotify123", "email", "name", "avatar", credentials);
        
        var query = new GetSpotifyTokenQuery(userId);
        _repositoryMock.GetByIdAsync(query.UserId).Returns(user);

        // Act
        var result = await _handler.Handle(query, _repositoryMock, _spotifyAuthServiceMock);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("access", result!.AccessToken);
        Assert.Equal(expiry, result.ExpiresAt);
        await _spotifyAuthServiceMock.DidNotReceiveWithAnyArgs().RefreshTokenAsync(default!);
    }

    [Fact]
    public async Task Handle_WithExpiredCredentials_RefreshesToken_AndUpdatesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var oldExpiry = DateTimeOffset.UtcNow.AddHours(-1);
        var oldCredentials = new SpotifyCredentials("oldAccess", "oldRefresh", oldExpiry);
        var user = new User(userId, "spotify123", "email", "name", "avatar", oldCredentials);
        
        var query = new GetSpotifyTokenQuery(userId);
        _repositoryMock.GetByIdAsync(query.UserId).Returns(user);

        var newExpiry = DateTimeOffset.UtcNow.AddHours(1);
        var newCredentials = new SpotifyCredentials("newAccess", "newRefresh", newExpiry);
        _spotifyAuthServiceMock.RefreshTokenAsync("oldRefresh").Returns(newCredentials);

        // Act
        var result = await _handler.Handle(query, _repositoryMock, _spotifyAuthServiceMock);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newAccess", result!.AccessToken);
        Assert.Equal(newExpiry, result.ExpiresAt);
        
        await _repositoryMock.Received(1).UpdateAsync(user);
        Assert.Equal("newAccess", user.Credentials!.AccessToken);
    }

    [Fact]
    public async Task Handle_WithExpiredCredentials_WhenRevoked_RevokesUserCredentials_AndReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var oldExpiry = DateTimeOffset.UtcNow.AddHours(-1);
        var oldCredentials = new SpotifyCredentials("oldAccess", "oldRefresh", oldExpiry);
        var user = new User(userId, "spotify123", "email", "name", "avatar", oldCredentials);
        
        var query = new GetSpotifyTokenQuery(userId);
        _repositoryMock.GetByIdAsync(query.UserId).Returns(user);

        _spotifyAuthServiceMock.RefreshTokenAsync("oldRefresh").ThrowsAsync(new RefreshTokenRevokedException());

        // Act
        var result = await _handler.Handle(query, _repositoryMock, _spotifyAuthServiceMock);

        // Assert
        Assert.Null(result);
        
        await _repositoryMock.Received(1).UpdateAsync(user);
        Assert.Null(user.Credentials);
    }
}
