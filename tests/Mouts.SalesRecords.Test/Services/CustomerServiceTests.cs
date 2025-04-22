using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Mouts.SalesRecords.Application.Core.Services;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Test.Services
{
    public class CustomerServiceTests
    {
        private readonly SalesContext _context;
        private readonly CustomerService _service;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;

        public CustomerServiceTests()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _service = new CustomerService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddCustomer()
        {
            var request = new CreateCustomerRequest
            {
                Name = "Cliente Teste",
                IsActive = true
            };

            await _service.CreateAsync(request);

            var customer = await _context.Customers.FirstOrDefaultAsync();
            Assert.NotNull(customer);
            Assert.Equal(request.Name, customer!.Name);
            Assert.True(customer.IsActive);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllCustomers()
        {
            _context.Customers.AddRange(
                new Customer { Id = Guid.NewGuid(), Name = "A", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Customer { Id = Guid.NewGuid(), Name = "B", CreatedAt = DateTime.UtcNow, IsActive = true }
            );
            await _context.SaveChangesAsync();

            var result = await _service.ReadAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnCustomer_WhenExists()
        {
            var customer = new Customer { Id = Guid.NewGuid(), Name = "Teste", CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var result = await _service.ReadAsync(customer.Id);

            Assert.NotNull(result);
            Assert.Equal("Teste", result!.Name);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnNull_WhenNotFound()
        {
            var result = await _service.ReadAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenCustomerIsUpdated()
        {
            var customer = new Customer { Id = Guid.NewGuid(), Name = "Original", CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            customer.Name = "Atualizado";

            var result = await _service.UpdateAsync(customer);

            Assert.True(result);
            var updated = await _context.Customers.FindAsync(customer.Id);
            Assert.Equal("Atualizado", updated!.Name);
            Assert.NotNull(updated.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            var fakeCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Fake",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _service.UpdateAsync(fakeCustomer);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_AndDeactivateCustomer()
        {
            var customer = new Customer { Id = Guid.NewGuid(), Name = "Teste", CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(customer.Id);

            Assert.True(result);
            var updated = await _context.Customers.FindAsync(customer.Id);
            Assert.False(updated!.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenCustomerNotFound()
        {
            var result = await _service.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }
}
