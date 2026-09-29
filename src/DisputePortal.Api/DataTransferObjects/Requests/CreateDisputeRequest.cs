namespace DisputePortal.Api.DataTransferObjects.Requests;

public class CreateDisputeRequest
{
    public Guid TransactionId { get; set; }
    public DisputeCategory Category { get; set; }
    public string Reason { get;  set; } = null!;
}