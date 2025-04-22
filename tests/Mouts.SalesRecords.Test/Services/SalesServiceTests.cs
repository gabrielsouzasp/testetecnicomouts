using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Domain.Responses;
using Mouts.SalesRecords.Application.Core.Services;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Test.Services
{
    public class SalesServiceTests
    {
        private readonly Mock<ILogger<SalesService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SalesContext _context;
        private readonly SalesService _service;

        public SalesServiceTests()
        {
            _loggerMock = new Mock<ILogger<SalesService>>();
            _mapperMock = new Mock<IMapper>();

            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new SalesContext(options);
            _service = new SalesService(_context, _loggerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenBranchDoesNotExist()
        {
            var request = new CreateSaleRequest
            {
                BranchId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Products = new List<CreateSaleProductsRequest>()
            };

            await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ShouldApply10PercentDiscount_WhenProductCountBetween4And9()
        {
            var branch = new Branch { Id = Guid.NewGuid(), Name = "Branch", CreatedAt = DateTime.UtcNow };
            var customer = new Customer { Id = Guid.NewGuid(), Name = "Customer", CreatedAt = DateTime.UtcNow };
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product", Quantity = 50, UnitPrice = 10m, IsActive = true, CreatedAt = DateTime.UtcNow };

            await _context.Branches.AddAsync(branch);
            await _context.Customers.AddAsync(customer);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var request = new CreateSaleRequest
            {
                BranchId = branch.Id,
                CustomerId = customer.Id,
                Products = Enumerable.Range(0, 5).Select(_ => new CreateSaleProductsRequest { Id = productId, Quantity = 1 }).ToList()
            };

            await _service.CreateAsync(request);
            var sale = await _context.Sales.FirstOrDefaultAsync();
            Assert.Equal(10, sale?.Discount);
        }

        [Fact]
        public async Task CreateAsync_ShouldApply20PercentDiscount_WhenProductCountBetween10And20()
        {
            var branch = new Branch { Id = Guid.NewGuid(), Name = "Branch", CreatedAt = DateTime.UtcNow };
            var customer = new Customer { Id = Guid.NewGuid(), Name = "Customer", CreatedAt = DateTime.UtcNow };
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product", Quantity = 100, UnitPrice = 10m, IsActive = true, CreatedAt = DateTime.UtcNow };

            await _context.Branches.AddAsync(branch);
            await _context.Customers.AddAsync(customer);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var request = new CreateSaleRequest
            {
                BranchId = branch.Id,
                CustomerId = customer.Id,
                Products = Enumerable.Range(0, 10).Select(_ => new CreateSaleProductsRequest { Id = productId, Quantity = 1 }).ToList()
            };

            await _service.CreateAsync(request);
            var sale = await _context.Sales.FirstOrDefaultAsync();
            Assert.Equal(20, sale?.Discount);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnMappedResponse_WhenSaleExists()
        {
            var sale = new Sale { Id = Guid.NewGuid(), BranchId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();

            var expectedResponse = new ReadSalesResponse();
            _mapperMock.Setup(m => m.Map<ReadSalesResponse>(It.IsAny<Sale>())).Returns(expectedResponse);

            var result = await _service.ReadAsync(sale.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenSaleIsUpdated()
        {
            var sale = new Sale { Id = Guid.NewGuid(), BranchId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();

            var request = new Sale { Id = sale.Id, BranchId = sale.BranchId, CustomerId = sale.CustomerId, Discount = 5, IsActive = true, IsCancelled = false };
            _mapperMock.Setup(m => m.Map<Sale>(request)).Returns(new Sale { Id = request.Id, BranchId = request.BranchId, CustomerId = request.CustomerId, Discount = request.Discount, IsActive = true, IsCancelled = false });

            var result = await _service.UpdateAsync(request);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenSaleIsDeleted()
        {
            var sale = new Sale { Id = Guid.NewGuid(), BranchId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(sale.Id);
            Assert.True(result);

            var deleted = await _context.Sales.FindAsync(sale.Id);
            Assert.False(deleted?.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenSaleDoesNotExist()
        {
            var result = await _service.DeleteAsync(Guid.NewGuid());
            Assert.False(result);
        }
    }
}