using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Application.Core.Services
{
    public class BranchService : IBranchService
    {
        private readonly SalesContext _context;
        private readonly ILogger<BranchService> _logger;

        public BranchService(SalesContext context,
            ILogger<BranchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var branch = new Branch
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = request.IsActive
                };

                await _context.Branches.AddAsync(branch, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<Branch>> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Branches.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<Branch?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Branches.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Branch request, CancellationToken cancellationToken = default)
        {
            try
            {
                var branch = await _context.Branches.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (branch is null) return false;

                _context.Entry(branch).CurrentValues.SetValues(request);
                branch.UpdatedAt = DateTime.UtcNow;
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
                var branch = await _context.Branches.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (branch is null) return false;

                branch.IsActive = false;
                _context.Branches.Update(branch);
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
