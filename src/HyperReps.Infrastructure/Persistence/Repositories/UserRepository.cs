using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HyperReps.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HyperRepsContext _context;

        public UserRepository(HyperRepsContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetBySpotifyIdAsync(string spotifyId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.SpotifyId == spotifyId);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
