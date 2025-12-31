using HyperReps.Application.Common.Interfaces.Persistence;
using HyperReps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HyperReps.Infrastructure.Persistence.Repositories
{
    public class MixRepository : IMixRepository
    {
        private readonly HyperRepsContext _context;

        public MixRepository(HyperRepsContext context)
        {
            _context = context;
        }

        public async Task<Mix?> GetByIdAsync(Guid id)
        {
            return await _context.Mixes
                .Include(m => m.Segments)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Mix mix)
        {
            await _context.Mixes.AddAsync(mix);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Mix mix)
        {
            _context.Mixes.Update(mix);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Mix mix)
        {
            _context.Mixes.Remove(mix);
            await _context.SaveChangesAsync();
        }
    }
}
