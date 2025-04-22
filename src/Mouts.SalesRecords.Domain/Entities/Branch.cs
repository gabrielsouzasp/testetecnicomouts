namespace Mouts.SalesRecords.Domain.Entities
{
    public class Branch
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
