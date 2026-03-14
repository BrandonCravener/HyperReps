using HyperReps.Domain.ValueObjects;

namespace HyperReps.Application.Common.Interfaces.Services
{
    public interface ISpotifyAuthService
    {
        Task<SpotifyCredentials> RefreshTokenAsync(string refreshToken);
    }
}
