namespace Mouts.SalesRecords.Domain.Entities
{
    public class SaleProduct
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
    }
}
