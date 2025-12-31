using HyperReps.Domain.Entities;

namespace HyperReps.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetBySpotifyIdAsync(string spotifyId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
