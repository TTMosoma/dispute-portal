using DisputePortal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace DisputePortal.Api.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(DisputePortalDbContext db)
        {
            // we will only seed an empty database
            if (await db.User.AnyAsync())
                return;
                
            var hasher = new PasswordHasher<User>();

            var joe = new User("joe@email.com", "Joe Soap", Role.Customer);
            joe.SetPasswordHash(hasher.HashPassword(joe, "Password123!"));

            var agent = new User("agent@abcbank.com", "James Bond", Role.Agent);
            agent.SetPasswordHash(hasher.HashPassword(agent, "Password123!"));

            var account = new Account(joe.Id, "ACC001");
            var transactions = new[]
            {
                new Transaction(account.Id, 250.00m, "ZAR", "Woolworths",  "Groceries", DateTimeOffset.UtcNow.AddDays(-5), TransactionType.Debit),
                new Transaction(account.Id, 5000.00m,"ZAR", "Salary",      "Monthly pay",DateTimeOffset.UtcNow.AddDays(-1), TransactionType.Credit),
            };

            db.AddRange(joe, agent, account);
            db.AddRange(transactions);
            await db.SaveChangesAsync();
        }
    }
}