namespace Mouts.SalesRecords.Domain.Responses
{
    public class ReadSalesResponse
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<ReadSalesProductsResponse>? SaleProducts { get; set; }
        public ReadSalesBranchResponse? Branch { get; set; }
        public ReadSalesCustomerResponse? Customer { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public int Discount { get; set; }
    }

    public class ReadSalesProductsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
    }

    public class ReadSalesBranchResponse
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ReadSalesCustomerResponse
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
