using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;

namespace Mouts.SalesRecords.Application.Core.Services.Interfaces
{
    public interface ICustomerService
    {
        Task CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
        Task<List<Customer>> ReadAllAsync(CancellationToken cancellationToken = default);
        Task<Customer?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Customer request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
