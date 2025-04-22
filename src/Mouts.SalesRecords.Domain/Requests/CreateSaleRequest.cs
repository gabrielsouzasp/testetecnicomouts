namespace Mouts.SalesRecords.Domain.Requests
{
    public class CreateSaleRequest
    {
        public Guid CustomerId { get; set; }
        public Guid BranchId { get; set; }
        public List<CreateSaleProductsRequest> Products { get; set; } = [];
    }

    public class CreateSaleProductsRequest
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }
}
