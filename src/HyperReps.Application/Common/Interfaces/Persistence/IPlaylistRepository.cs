using HyperReps.Domain.Entities;

namespace HyperReps.Application.Common.Interfaces.Persistence
{
    public interface IPlaylistRepository
    {
        Task<Playlist?> GetByIdAsync(Guid id);
        Task AddAsync(Playlist playlist);
        Task UpdateAsync(Playlist playlist);
        Task DeleteAsync(Playlist playlist);
    }
}
