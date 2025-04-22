namespace Mouts.SalesRecords.Domain.Requests
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
