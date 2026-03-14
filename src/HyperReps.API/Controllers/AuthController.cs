using System.Security.Claims;
using System.Text.Json;
using HyperReps.Application.Users.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;
using Wolverine;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace HyperReps.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMessageBus _bus;
        private readonly ILogger<AuthController> _logger;

        private record SpotifyImage(string Url, int? Height, int? Width);

        public AuthController(IMessageBus bus, ILogger<AuthController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpGet("login")]
        public async Task<ActionResult> Login(string redirectUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.IsLocalUrl(redirectUrl) ? redirectUrl : Url.Action(),
            };
            return Challenge(properties, OpenIddictClientWebIntegrationConstants.Providers.Spotify);
        }

        [HttpGet("callback"), IgnoreAntiforgeryToken]
        public async Task<ActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync(
                OpenIddictClientAspNetCoreDefaults.AuthenticationScheme
            );

            if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
                throw new InvalidOperationException(
                    "The external authorization data cannot be used for authentication."
                );

            if (result.Properties is null)
                throw new InvalidOperationException(
                    "Authentication properties are missing from the external authentication result."
                );

            // 1. Extract the data needed for the Domain
            var spotifyId = result.Principal.GetClaim(ClaimTypes.NameIdentifier)!;
            var email = result.Principal.GetClaim(ClaimTypes.Email) ?? "";
            var name = result.Principal.GetClaim(ClaimTypes.Name) ?? "Unknown User";
            var rawPictureJson = result.Principal.FindFirstValue("images");

            string avatarUrl = "";

            if (!string.IsNullOrWhiteSpace(rawPictureJson))
            {
                try
                {
                    var images = JsonSerializer.Deserialize<SpotifyImage>(
                        rawPictureJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    avatarUrl = images?.Url ?? "";
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Failed to deserialize Spotify images JSON. Raw value: {RawJson}",
                        rawPictureJson
                    );

                    avatarUrl = "";
                }
            }

            // Extract the tokens OpenIddict received from the provider
            var accessToken = result.Properties.GetTokenValue(
                OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken
            );
            var refreshToken = result.Properties.GetTokenValue(
                OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken
            );

            // OpenIddict stores expiration in the properties. Fallback to 1 hour if missing.
            var expiresAtString = result.Properties.GetString(".Token.expires_at");
            var expiry = DateTimeOffset.TryParse(expiresAtString, out var parsed)
                ? parsed
                : DateTimeOffset.UtcNow.AddHours(1);

            // 2. Dispatch the Command to Wolverine
            var command = new UpsertUserCommand(
                spotifyId,
                email,
                name,
                avatarUrl,
                accessToken ?? "",
                refreshToken ?? "",
                expiry
            );
            var internalUserId = await _bus.InvokeAsync<Guid>(command);

            // 3. Build the local Identity
            var identity = new ClaimsIdentity(authenticationType: "ExternalLogin");

            identity
                .SetClaim(ClaimTypes.Email, email)
                .SetClaim(ClaimTypes.Name, name)
                // Save the internal Database ID as the primary identifier for your API
                .SetClaim(ClaimTypes.NameIdentifier, internalUserId.ToString())
                // Save the original Spotify ID as a secondary claim
                .SetClaim("SpotifyId", spotifyId)
                .SetClaim(
                    Claims.Private.RegistrationId,
                    result.Principal.GetClaim(Claims.Private.RegistrationId)
                );

            var properties = new AuthenticationProperties(result.Properties.Items)
            {
                RedirectUri = result.Properties.RedirectUri ?? "/",
            };

            return SignIn(new ClaimsPrincipal(identity), properties);
        }
    }
}
