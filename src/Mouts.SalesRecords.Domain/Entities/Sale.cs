namespace Mouts.SalesRecords.Domain.Entities
{
    public class Sale
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CustomerId { get; set; } 
        public Guid BranchId { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public int Discount { get; set; }
    }
}
