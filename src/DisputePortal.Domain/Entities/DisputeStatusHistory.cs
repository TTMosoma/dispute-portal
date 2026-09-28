public class DisputeStatusHistory
{
    private DisputeStatusHistory() { }
    public DisputeStatusHistory(Guid disputeId, DisputeStatus? fromStatus, DisputeStatus toStatus, Guid changedBy, string? note, DateTimeOffset? changedAt)
    {
        Id = Guid.NewGuid();
        DisputeId = disputeId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedBy = changedBy;
        Note = note;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid DisputeId { get; private set; }
    public Guid ChangedBy { get; private set; }
    public DisputeStatus? FromStatus { get; private set; }
    public DisputeStatus ToStatus { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }
    public string? Note { get; private set; }
}