
public class InvalidDisputeTransitionException : DomainException
{
    public string TransitionGuid { get; }
    public InvalidDisputeTransitionException(string message, Guid transitionGuid)
        : base(message)
    {
        TransitionGuid = transitionGuid.ToString();
    }
}