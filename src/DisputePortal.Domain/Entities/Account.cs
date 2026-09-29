namespace Dispute.Domain.Entities
{

    public class Account
    {
        private Account() { }
        public Account(Guid userId, string accountNumber)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            AccountNumber = accountNumber;
        }
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string AccountNumber { get; private set; } = null!;
    }
}