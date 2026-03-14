using HyperReps.Domain.Entities;

namespace HyperReps.Application.Common.Interfaces.Persistence
{
    public interface ITrackRepository
    {
        Task<Track?> GetBySpotifyIdAsync(string spotifyId);
        Task AddAsync(Track track);
    }
}
