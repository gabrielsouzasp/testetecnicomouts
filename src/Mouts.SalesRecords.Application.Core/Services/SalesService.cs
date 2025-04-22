using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;
using Mouts.SalesRecords.Domain.Responses;
using Mouts.SalesRecords.Infra.Database;

namespace Mouts.SalesRecords.Application.Core.Services
{
    public class SalesService : ISalesService
    {
        private readonly SalesContext _context;
        private readonly ILogger<SalesService> _logger;
        private readonly IMapper _mapper;

        public SalesService(SalesContext context,
            ILogger<SalesService> logger,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                Guid saleId = Guid.NewGuid();

                var branch = await _context.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.BranchId, cancellationToken) ??
                    throw new Exception("Branch does not exist!");

                var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken) ??
                    throw new Exception("Customer does not exist!");

                var sale = new Sale
                {
                    BranchId = request.BranchId,
                    CreatedAt = DateTime.UtcNow,
                    CustomerId = request.CustomerId,
                    Id = saleId,
                    IsCancelled = false,
                    IsActive = true
                };

                if (request.Products is { Count: >= 4 and < 10 })
                    sale.Discount = 10;

                if (request.Products is { Count: >= 10 and <= 20 })
                    sale.Discount = 20;

                foreach (var productRequest in request.Products)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productRequest.Id, cancellationToken) ??
                        throw new Exception("Product does not exist!");

                    var quantity = productRequest.Quantity > 20 ? 20 : productRequest.Quantity;

                    if (product.Quantity < productRequest.Quantity)
                        quantity = product.Quantity;

                    product.Quantity -= quantity;

                    await _context.SaleProducts.AddAsync(new SaleProduct
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productRequest.Id,
                        Quantity = quantity,
                        SaleId = saleId,
                        UnitPrice = product.UnitPrice
                    }, cancellationToken);

                    _context.Products.Update(product);
                }

                await _context.Sales.AddAsync(sale, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<ReadSalesResponse>> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var sales = await _context.Sales
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var branchIds = sales.Select(s => s.BranchId).Distinct();
                var customerIds = sales.Select(s => s.CustomerId).Distinct();
                var saleIds = sales.Select(s => s.Id).Distinct();

                var branches = await _context.Branches
                    .AsNoTracking()
                    .Where(b => branchIds.Contains(b.Id))
                    .ToDictionaryAsync(b => b.Id, cancellationToken);

                var customers = await _context.Customers
                    .AsNoTracking()
                    .Where(c => customerIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, cancellationToken);

                var saleProducts = await _context.SaleProducts
                    .AsNoTracking()
                    .Where(sp => saleIds.Contains(sp.SaleId))
                    .Include(sp => sp.Product)
                    .ToListAsync(cancellationToken);

                var groupedSaleProducts = saleProducts
                    .GroupBy(sp => sp.SaleId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var responses = new List<ReadSalesResponse>();

                foreach (var sale in sales)
                {
                    var response = _mapper.Map<ReadSalesResponse>(sale);

                    if (groupedSaleProducts.TryGetValue(sale.Id, out var spList))
                        response.SaleProducts = spList.Select(sp => new ReadSalesProductsResponse
                        {
                            Id = sp.ProductId,
                            Name = sp.Product?.Name ?? string.Empty,
                            Quantity = sp.Quantity,
                            UnitPrice = sp.UnitPrice
                        });
                    
                    if (branches.TryGetValue(sale.BranchId, out var branch))
                        response.Branch = new()
                        {
                            Id = branch.Id,
                            Name = branch.Name
                        };

                    if (customers.TryGetValue(sale.CustomerId, out var customer))
                        response.Customer = new()
                        {
                            Id = customer.Id,
                            Name = customer.Name
                        };

                    responses.Add(response);
                }

                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }


        public async Task<ReadSalesResponse?> ReadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var sale = await _context.Sales
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (sale is null)
                    return null;

                var branchTask = _context.Branches
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == sale.BranchId, cancellationToken);

                var customerTask = _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == sale.CustomerId, cancellationToken);

                var saleProductsTask = _context.SaleProducts
                    .AsNoTracking()
                    .Where(x => x.SaleId == sale.Id)
                    .Include(x => x.Product)
                    .ToListAsync(cancellationToken);

                var branch = await branchTask;
                var customer = await customerTask;
                var saleProducts = await saleProductsTask;
                var response = _mapper.Map<ReadSalesResponse>(sale);

                response.SaleProducts = saleProducts.Select(sp => new ReadSalesProductsResponse
                {
                    Id = sp.ProductId,
                    Name = sp.Product?.Name ?? string.Empty,
                    Quantity = sp.Quantity,
                    UnitPrice = sp.UnitPrice
                });

                if (branch is not null)
                    response.Branch = new()
                    {
                        Id = branch.Id,
                        Name = branch.Name
                    };

                if (customer is not null)
                    response.Customer = new()
                    {
                        Id = customer.Id,
                        Name = customer.Name
                    };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Sale request, CancellationToken cancellationToken = default)
        {
            try
            {
                var sale = await _context.Sales.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (sale is null) return false;

                _context.Entry(sale).CurrentValues.SetValues(request);
                sale.UpdatedAt = DateTime.UtcNow;
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
                var sale = await _context.Sales.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (sale is null) return false;

                sale.IsActive = false;
                _context.Sales.Update(sale);
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
