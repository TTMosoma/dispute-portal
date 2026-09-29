namespace DisputePortal.Domain.Entities;

public class Transaction
{
    private Transaction() { }
    public Transaction(Guid accountId, decimal amount, string currency, string merchantName, string description, DateTimeOffset transactionDate, TransactionType transactionType)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Amount = amount;
        Currency = currency;
        MerchantName = merchantName;
        Description = description;
        TransactionDate = transactionDate;
        TransactionType = transactionType;
    }

    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    public string MerchantName { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTimeOffset TransactionDate { get; private set; }
    public TransactionType TransactionType { get; private set; }
}
