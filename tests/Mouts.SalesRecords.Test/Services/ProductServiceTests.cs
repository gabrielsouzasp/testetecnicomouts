using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Mouts.SalesRecords.Application.Core.Services;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Test.Services
{
    public class ProductServiceTests
    {
        private readonly ProductService _service;
        private readonly SalesContext _context;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SalesContext(options);
            var loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_context, loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddProduct()
        {
            var request = new CreateProductRequest
            {
                Name = "Test Product",
                IsActive = true,
                Quantity = 10,
                UnitPrice = 99.99m
            };

            await _service.CreateAsync(request);

            var product = await _context.Products.FirstOrDefaultAsync();
            Assert.NotNull(product);
            Assert.Equal("Test Product", product!.Name);
            Assert.Equal(10, product.Quantity);
            Assert.Equal(99.99m, product.UnitPrice);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllProducts()
        {
            _context.Products.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test 1",
                IsActive = true,
                Quantity = 5,
                UnitPrice = 10,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var result = await _service.ReadAllAsync();

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnProduct_WhenExists()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "ToRead",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Quantity = 1,
                UnitPrice = 10m
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var result = await _service.ReadAsync(product.Id);
            Assert.NotNull(result);
            Assert.Equal("ToRead", result!.Name);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnNull_WhenNotFound()
        {
            var result = await _service.ReadAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenExists()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Quantity = 1,
                UnitPrice = 5
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var updated = new Product
            {
                Id = product.Id,
                Name = "New Name",
                IsActive = false,
                Quantity = 100,
                UnitPrice = 55m
            };

            var result = await _service.UpdateAsync(updated);
            var productUpdated = await _context.Products.FindAsync(product.Id);

            Assert.True(result);
            Assert.Equal("New Name", productUpdated?.Name);
            Assert.Equal(100, productUpdated?.Quantity);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenProductNotExists()
        {
            var result = await _service.UpdateAsync(new Product { Id = Guid.NewGuid() });
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSetProductInactive_WhenExists()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "ToDelete",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Quantity = 5,
                UnitPrice = 20m
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var result = await _service.DeleteAsync(product.Id);

            var softDeleted = await _context.Products.FindAsync(product.Id);

            Assert.True(result);
            Assert.False(softDeleted!.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotExists()
        {
            var result = await _service.DeleteAsync(Guid.NewGuid());
            Assert.False(result);
        }
    }
}
