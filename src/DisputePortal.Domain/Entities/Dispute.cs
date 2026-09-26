public class Dispute
{
    private readonly List<DisputeStatusHistory> _statusHistory = new();
    private Dispute() { }
    public Dispute(Guid transactionId, Guid customerId, DisputeCategory category, string reason)
    {
        Id = Guid.NewGuid();
        CreatedAt = UpdatedAt = DateTimeOffset.UtcNow;
        Status = DisputeStatus.Submitted;
        CustomerId = customerId;
        Category = category;
        Reason = reason;
        TransactionId = transactionId;
    }

    public Guid Id { get; private set; }
    public Guid TransactionId { get; private set; }
    public Guid CustomerId { get; private set; }
    public DisputeCategory Category { get; private set; }
    public string Reason { get; private set; } = null!;
    public DisputeStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<DisputeStatusHistory> StatusHistory { get { return _statusHistory; } }
}