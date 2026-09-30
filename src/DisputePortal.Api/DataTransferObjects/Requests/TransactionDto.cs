namespace DisputePortal.Api.DataTransferObjects.Requests;

public class TransactionDto
{
    public required Guid Id { get; set; }
    public required decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string MerchantName { get; set; }
    public DateTimeOffset TransactionDate { get; set; }
    public required string Description { get; set; }
    public required string TransactionType { get; set; }
}