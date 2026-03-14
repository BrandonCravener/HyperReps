using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HyperReps.API.Controllers;
using HyperReps.Application.Users.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;
using Wolverine;
using Xunit;

namespace HyperReps.UnitTests.API.Controllers;

public class AuthControllerTests
{
    private readonly IMessageBus _busMock;
    private readonly ILogger<AuthController> _loggerMock;
    private readonly AuthController _controller;
    private readonly IUrlHelper _urlHelperMock;
    private readonly DefaultHttpContext _httpContext;
    private readonly IAuthenticationService _authServiceMock;

    public AuthControllerTests()
    {
        _busMock = Substitute.For<IMessageBus>();
        _loggerMock = Substitute.For<ILogger<AuthController>>();

        _authServiceMock = Substitute.For<IAuthenticationService>();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(_authServiceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        _httpContext = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        _urlHelperMock = Substitute.For<IUrlHelper>();
        _urlHelperMock.IsLocalUrl(Arg.Any<string>()).Returns(CallInfo => CallInfo.Arg<string>().StartsWith("/"));
        _urlHelperMock.Action(Arg.Any<UrlActionContext>()).Returns("/default-url");

        _controller = new AuthController(_busMock, _loggerMock)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            },
            Url = _urlHelperMock
        };
    }

    [Fact]
    public async Task Login_WithLocalUrl_ReturnsChallengeWithLocalUrl()
    {
        // Arrange
        var redirectUrl = "/local-route";

        // Act
        var result = await _controller.Login(redirectUrl);

        // Assert
        var challengeResult = Assert.IsType<ChallengeResult>(result);
        Assert.Contains(OpenIddictClientWebIntegrationConstants.Providers.Spotify, challengeResult.AuthenticationSchemes);
        Assert.Equal(redirectUrl, challengeResult.Properties!.RedirectUri);
    }

    [Fact]
    public async Task Login_WithExternalUrl_ReturnsChallengeWithDefaultActionUrl()
    {
        // Arrange
        var redirectUrl = "http://external.com";

        // Act
        var result = await _controller.Login(redirectUrl);

        // Assert
        var challengeResult = Assert.IsType<ChallengeResult>(result);
        Assert.Equal("/default-url", challengeResult.Properties!.RedirectUri);
    }
}
