namespace Mouts.SalesRecords.Domain.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; }
    }
}
