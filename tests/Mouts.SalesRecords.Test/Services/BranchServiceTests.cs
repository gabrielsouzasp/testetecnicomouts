using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Mouts.SalesRecords.Application.Core.Services;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Test.Services
{
    public class BranchServiceTests
    {
        private readonly SalesContext _context;
        private readonly BranchService _service;
        private readonly Mock<ILogger<BranchService>> _loggerMock;

        public BranchServiceTests()
        {
            var options = new DbContextOptionsBuilder<SalesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new SalesContext(options);
            _loggerMock = new Mock<ILogger<BranchService>>();
            _service = new BranchService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddBranch()
        {
            var request = new CreateBranchRequest
            {
                Name = "Filial Teste",
                IsActive = true
            };

            await _service.CreateAsync(request);

            var branch = await _context.Branches.FirstOrDefaultAsync();
            Assert.NotNull(branch);
            Assert.Equal(request.Name, branch!.Name);
            Assert.True(branch.IsActive);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllBranches()
        {
            _context.Branches.AddRange(
                new Branch { Id = Guid.NewGuid(), Name = "A", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Branch { Id = Guid.NewGuid(), Name = "B", CreatedAt = DateTime.UtcNow, IsActive = true }
            );
            await _context.SaveChangesAsync();

            var result = await _service.ReadAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnBranch_WhenExists()
        {
            var branch = new Branch { Id = Guid.NewGuid(), Name = "Centro", CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            var result = await _service.ReadAsync(branch.Id);

            Assert.NotNull(result);
            Assert.Equal("Centro", result!.Name);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnNull_WhenNotFound()
        {
            var result = await _service.ReadAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenBranchExists()
        {
            var branch = new Branch { Id = Guid.NewGuid(), Name = "Original", CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            branch.Name = "Atualizado";

            var result = await _service.UpdateAsync(branch);

            Assert.True(result);
            var updated = await _context.Branches.FindAsync(branch.Id);
            Assert.Equal("Atualizado", updated!.Name);
            Assert.NotNull(updated.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenBranchNotFound()
        {
            var fakeBranch = new Branch
            {
                Id = Guid.NewGuid(),
                Name = "Falsa",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _service.UpdateAsync(fakeBranch);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_AndDeactivateBranch()
        {
            var branch = new Branch { Id = Guid.NewGuid(), Name = "Teste", CreatedAt = DateTime.UtcNow, IsActive = true };
            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(branch.Id);

            Assert.True(result);
            var updated = await _context.Branches.FindAsync(branch.Id);
            Assert.False(updated!.IsActive);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenBranchNotFound()
        {
            var result = await _service.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }
}
