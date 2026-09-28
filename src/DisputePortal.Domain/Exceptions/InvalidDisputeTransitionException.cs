
public class InvalidDisputeTransitionException : DomainException
{
    public InvalidDisputeTransitionException(DisputeStatus from, DisputeStatus to, Role role)
        : base($"Cannot move dispute from {from} to {to} as {role}.")
    {
    }
}