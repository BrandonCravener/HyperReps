using HyperReps.Domain.Entities;

namespace HyperReps.Application.Common.Interfaces.Persistence
{
    public interface IMixRepository
    {
        Task<Mix?> GetByIdAsync(Guid id);
        Task AddAsync(Mix mix);
        Task UpdateAsync(Mix mix);
        Task DeleteAsync(Mix mix);
    }
}
