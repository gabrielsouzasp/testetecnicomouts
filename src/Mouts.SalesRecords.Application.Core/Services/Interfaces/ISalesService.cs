using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Domain.Responses;

namespace Mouts.SalesRecords.Application.Core.Services.Interfaces
{
    public interface ISalesService
    {
        Task CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default);
        Task<List<ReadSalesResponse>> ReadAllAsync(CancellationToken cancellationToken = default);
        Task<ReadSalesResponse?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Sale request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
