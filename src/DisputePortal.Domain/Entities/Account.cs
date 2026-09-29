public class Account
{
    private Account() { }
    public Account(Guid customerId, string accountNumber)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        AccountNumber = accountNumber;
    }
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string AccountNumber { get; private set; } = null!;
}