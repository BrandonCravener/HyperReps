using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Application.Common.Interfaces.Services;
using HyperReps.Domain.Exceptions;

namespace HyperReps.Application.Users.Queries
{
    public record GetSpotifyTokenQuery(Guid UserId);

    public record SpotifyTokenResponse(string AccessToken, DateTimeOffset ExpiresAt);

    public class GetSpotifyTokenHandler
    {
        public async Task<SpotifyTokenResponse?> Handle(
            GetSpotifyTokenQuery query,
            IUserRepository repository,
            ISpotifyAuthService spotifyAuthService
        )
        {
            var user = await repository.GetByIdAsync(query.UserId);

            if (user?.Credentials is null)
                return null;

            if (user.Credentials.IsExpired())
            {
                try
                {
                    var freshCredentials = await spotifyAuthService.RefreshTokenAsync(
                        user.Credentials.RefreshToken
                    );
                    user.UpdateCredentials(freshCredentials);
                    await repository.UpdateAsync(user);
                }
                catch (RefreshTokenRevokedException)
                {
                    user.RevokeCredentials();
                    await repository.UpdateAsync(user);
                    return null;
                }
            }

            return new SpotifyTokenResponse(user.Credentials.AccessToken, user.Credentials.Expiry);
        }
    }
}
