using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;

namespace Mouts.SalesRecords.Application.Core.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
        Task<List<Product>> ReadAllAsync(CancellationToken cancellationToken = default);
        Task<Product?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Product request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    }
}
