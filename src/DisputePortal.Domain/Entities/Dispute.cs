/// <summary>
/// This is the model that represent disputes, 
/// I have chosen the below rules to govern disputes.
/// Rule 1: A customer can withdraw a submitted dispute.
/// Rule 2: A customer can  withdraw a dispute that is under review.
/// Rule 3: An agent can resolve a dispute that is under review.
/// Rule 4: An agent can reject a dispute that is under review.
/// </summary>
public class Dispute
{
    private readonly List<DisputeStatusHistory> _statusHistory = new();
    /// <summary>
    /// Allowed transactions are whitelisted, in matrics format.
    /// This follows the established dispute business rule.
    /// i.e. An agent can move a submitted claim to under review. A client can withdraw a submitted claim, etc. 
    /// </summary>
    private static readonly HashSet<(DisputeStatus From, DisputeStatus To, Role Role)> _allowedTransitions = new()
{
    (DisputeStatus.Submitted,   DisputeStatus.UnderReview, Role.Agent),
    (DisputeStatus.Submitted,   DisputeStatus.Withdrawn,   Role.Customer),
    (DisputeStatus.UnderReview, DisputeStatus.Resolved,    Role.Agent),
    (DisputeStatus.UnderReview, DisputeStatus.Rejected,    Role.Agent),
    (DisputeStatus.UnderReview, DisputeStatus.Withdrawn,   Role.Customer),
};
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

        // To keep track of each dispute trail, we must log the first instance of a dispute, 
        // having null as the from status will signal the disput as a 'new' entry.
        AddHistory(null, DisputeStatus.Submitted, customerId, null, CreatedAt);
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

    public void MoveToUnderReview(Guid changedBy, string? note = null) => Transition(DisputeStatus.UnderReview, Role.Agent, changedBy, note);
    public void Resolve(Guid changedBy, string? note = null) => Transition(DisputeStatus.Resolved, Role.Agent, changedBy, note);
    public void Reject(Guid changedBy, string? note = null) => Transition(DisputeStatus.Rejected, Role.Agent, changedBy, note);
    public void Withdraw(Guid changedBy, string? note = null) => Transition(DisputeStatus.Withdrawn, Role.Customer, changedBy, note);

    private void AddHistory(DisputeStatus? from, DisputeStatus to, Guid changedBy, string? note, DateTimeOffset time)
    => _statusHistory.Add(new DisputeStatusHistory(Id, from, to, changedBy, note, time));

    /// <summary>
    /// Manages the various status of a dispute.
    ///     /// </summary>
    /// <param name="toStatus">The dispute status to change to</param>
    /// <param name="actingUserRole">The user</param>
    /// <param name="changedBy">the user ID of the person who is conducting the action</param>
    /// <param name="note">optional reason for this action</param>
    /// <exception cref="InvalidDisputeTransitionException">Thrown if an action by an actor is not a whitelisted action</exception>
    private void Transition(DisputeStatus toStatus, Role actingUserRole, Guid changedBy, string? note = null)
    {
        if (!_allowedTransitions.Contains((Status, toStatus, actingUserRole)))
            throw new InvalidDisputeTransitionException(Status, toStatus, actingUserRole);

        DateTimeOffset now = DateTimeOffset.UtcNow;
        AddHistory(Status, toStatus, changedBy, note, now);
        Status = toStatus;
        UpdatedAt = now;
    }
}