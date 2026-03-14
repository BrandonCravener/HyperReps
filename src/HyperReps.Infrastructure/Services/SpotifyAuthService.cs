using HyperReps.Application.Common.Interfaces.Services;
using HyperReps.Domain.Exceptions;
using HyperReps.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;

namespace HyperReps.Infrastructure.Services
{
    public class SpotifyAuthService : ISpotifyAuthService
    {
        private readonly IConfiguration _configuration;

        public SpotifyAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SpotifyCredentials> RefreshTokenAsync(string refreshToken)
        {
            var clientId = _configuration["Spotify:ClientId"]!;
            var clientSecret = _configuration["Spotify:ClientSecret"];

            try
            {
                // The SDK's OAuthClient handles the entire transaction
                var response = await new OAuthClient().RequestToken(
                    request: new PKCETokenRefreshRequest(clientId, refreshToken)
                );

                var newRefreshToken = response.RefreshToken ?? refreshToken;
                var expiry = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);

                return new SpotifyCredentials(response.AccessToken, newRefreshToken, expiry);
            }
            catch (APIException ex) when (ex.Message.Contains("invalid_grant"))
            {
                // The SDK throws an APIException on HTTP failures
                throw new RefreshTokenRevokedException();
            }
        }
    }
}
