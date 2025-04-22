using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Application.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly SalesContext _context;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(SalesContext context,
            ILogger<CustomerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = request.IsActive
                };

                await _context.Customers.AddAsync(product, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<Customer>> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Customers.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<Customer?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Customer request, CancellationToken cancellationToken = default)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (customer is null) return false;

                _context.Entry(customer).CurrentValues.SetValues(request);
                customer.UpdatedAt = DateTime.UtcNow;
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
                var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (customer is null) return false;

                customer.IsActive = false;
                _context.Customers.Update(customer);
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
