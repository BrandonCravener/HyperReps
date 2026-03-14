using System;
using System.Threading.Tasks;
using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Application.Users.Commands;
using HyperReps.Domain.Entities;
using HyperReps.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace HyperReps.UnitTests.Application.Users.Commands;

public class UpsertUserHandlerTests
{
    private readonly IUserRepository _repositoryMock;
    private readonly UpsertUserHandler _handler;

    public UpsertUserHandlerTests()
    {
        _repositoryMock = Substitute.For<IUserRepository>();
        _handler = new UpsertUserHandler();
    }

    [Fact]
    public async Task Handle_WithNewUser_CreatesAndAddsUser()
    {
        // Arrange
        var command = new UpsertUserCommand(
            "spotify123",
            "test@example.com",
            "Test Setup",
            "http://avatar.url",
            "access",
            "refresh",
            DateTimeOffset.UtcNow.AddHours(1)
        );

        _repositoryMock.GetBySpotifyIdAsync(command.SpotifyId).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, _repositoryMock);

        // Assert
        await _repositoryMock.Received(1).AddAsync(Arg.Is<User>(u =>
            u.SpotifyId == command.SpotifyId &&
            u.Email == command.Email &&
            u.DisplayName == command.DisplayName &&
            u.AvatarUrl == command.AvatarUrl &&
            u.Credentials!.AccessToken == command.AccessToken &&
            u.Credentials.RefreshToken == command.RefreshToken &&
            u.Credentials.Expiry == command.Expiry
        ));
        await _repositoryMock.DidNotReceive().UpdateAsync(Arg.Any<User>());
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_WithExistingUser_UpdatesUser()
    {
        // Arrange
        var expiry = DateTimeOffset.UtcNow.AddHours(1);
        var existingUser = new User(
            Guid.NewGuid(),
            "spotify123",
            "old@example.com",
            "Old Name",
            "http://old.avatar.url",
            new SpotifyCredentials("oldAccess", "oldRefresh", DateTimeOffset.UtcNow)
        );

        var command = new UpsertUserCommand(
            "spotify123",
            "new@example.com",
            "New Name",
            "http://new.avatar.url",
            "newAccess",
            "newRefresh",
            expiry
        );

        _repositoryMock.GetBySpotifyIdAsync(command.SpotifyId).Returns(existingUser);

        // Act
        var result = await _handler.Handle(command, _repositoryMock);

        // Assert
        await _repositoryMock.DidNotReceive().AddAsync(Arg.Any<User>());
        await _repositoryMock.Received(1).UpdateAsync(Arg.Is<User>(u =>
            u.Id == existingUser.Id &&
            u.SpotifyId == command.SpotifyId &&
            u.Email == command.Email &&
            u.DisplayName == command.DisplayName &&
            u.AvatarUrl == command.AvatarUrl &&
            u.Credentials!.AccessToken == command.AccessToken &&
            u.Credentials.RefreshToken == command.RefreshToken &&
            u.Credentials.Expiry == command.Expiry
        ));
        Assert.Equal(existingUser.Id, result);
    }
}
