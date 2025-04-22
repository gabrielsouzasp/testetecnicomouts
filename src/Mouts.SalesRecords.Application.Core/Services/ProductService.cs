using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Application.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly SalesContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(SalesContext context, 
            ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = request.IsActive,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice
                };

                await _context.Products.AddAsync(product, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<Product>> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Products.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<Product?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Product request, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (product is null) return false;

                _context.Entry(product).CurrentValues.SetValues(request);
                product.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (product is null) return false;

                product.IsActive = false;
                _context.Products.Update(product);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
