namespace DisputePortal.Api.DataTransferObjects.Responses;

public class HistoryDto
{
    public HistoryDto(DisputeStatus? fromStatus, DisputeStatus toStatus, Guid changedBy, string? note, DateTimeOffset changedAt)
    {
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedBy = changedBy;
        Note = note;
        ChangedAt = changedAt;
    }

    public DisputeStatus? FromStatus { get; }
    public DisputeStatus ToStatus { get; }
    public Guid ChangedBy { get; }
    public string? Note { get; }
    public DateTimeOffset ChangedAt { get; }
}