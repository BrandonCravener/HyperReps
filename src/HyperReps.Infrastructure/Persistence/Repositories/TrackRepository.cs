using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HyperReps.Infrastructure.Persistence.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly HyperRepsContext _context;

        public TrackRepository(HyperRepsContext context)
        {
            _context = context;
        }

        public async Task<Track?> GetBySpotifyIdAsync(string spotifyId)
        {
            return await _context.Tracks.FirstOrDefaultAsync(t => t.SpotifyTrackId == spotifyId);
        }

        public async Task AddAsync(Track track)
        {
            await _context.Tracks.AddAsync(track);
            await _context.SaveChangesAsync();
        }
    }
}
