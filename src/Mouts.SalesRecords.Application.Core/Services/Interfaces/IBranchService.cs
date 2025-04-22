using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;

namespace Mouts.SalesRecords.Application.Core.Services.Interfaces
{
    public interface IBranchService
    {
        Task CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default);
        Task<List<Branch>> ReadAllAsync(CancellationToken cancellationToken = default);
        Task<Branch?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Branch request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
