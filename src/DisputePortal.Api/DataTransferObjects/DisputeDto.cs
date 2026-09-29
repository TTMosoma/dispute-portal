namespace DisputePortal.Api.DataTransferObjects;

public class DisputeDto
{
    public DisputeDto(Guid id, Guid transactionId, string category, string status, string reason, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        Id = id;
        TransactionId = transactionId;
        Category = category;
        Status = status;
        Reason = reason;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string Category { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}