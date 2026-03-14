using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HyperReps.API.Controllers;
using HyperReps.Application.Users.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Wolverine;
using Xunit;

namespace HyperReps.UnitTests.API.Controllers;

public class UsersControllerTests
{
    private readonly IMessageBus _busMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _busMock = Substitute.For<IMessageBus>();
        _controller = new UsersController(_busMock);
    }

    private void SetUser(string? nameIdentifier)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier ?? string.Empty)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetMySpotifyToken_WithNoUserId_ReturnsUnauthorized()
    {
        // Arrange
        SetUser(null);

        // Act
        var result = await _controller.GetMySpotifyToken();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetMySpotifyToken_WithInvalidUserId_ReturnsUnauthorized()
    {
        // Arrange
        SetUser("not-a-guid");

        // Act
        var result = await _controller.GetMySpotifyToken();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetMySpotifyToken_WithNullTokenFromBus_Returns403()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUser(userId.ToString());
        _busMock.InvokeAsync<SpotifyTokenResponse?>(Arg.Any<GetSpotifyTokenQuery>())
            .Returns((SpotifyTokenResponse?)null);

        // Act
        var result = await _controller.GetMySpotifyToken();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(403, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task GetMySpotifyToken_WithValidToken_ReturnsOkWithToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUser(userId.ToString());
        var tokenResponse = new SpotifyTokenResponse("access_token", DateTimeOffset.UtcNow.AddHours(1));
        
        _busMock.InvokeAsync<SpotifyTokenResponse?>(Arg.Is<GetSpotifyTokenQuery>(q => q.UserId == userId))
            .Returns(tokenResponse);

        // Act
        var result = await _controller.GetMySpotifyToken();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedToken = Assert.IsType<SpotifyTokenResponse>(okResult.Value);
        Assert.Equal("access_token", returnedToken.AccessToken);
    }
}
