using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HyperReps.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly HyperRepsContext _context;

        public PlaylistRepository(HyperRepsContext context)
        {
            _context = context;
        }

        public async Task<Playlist?> GetByIdAsync(Guid id)
        {
            return await _context.Playlists
                .Include(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Playlist playlist)
        {
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }
    }
}
