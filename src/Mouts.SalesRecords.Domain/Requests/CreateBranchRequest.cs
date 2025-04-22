namespace Mouts.SalesRecords.Domain.Requests
{
    public class CreateBranchRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
