public class DisputeStatusHistory
{
    public Guid Id { get; set; }
    public required Guid DisputeId { get; set; }
    public required Guid ChangedBy { get; set; }
    public DisputeStatus? FromStatus { get; set; }
    public DisputeStatus ToStatus { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public string? Note { get; set; }
}