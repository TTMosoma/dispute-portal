using Microsoft.EntityFrameworkCore;

namespace DisputePortal.Infrastructure.Persistence
{
    public class DisputePortalDbContext : DbContext
    {
        public DisputePortalDbContext(DbContextOptions<DisputePortalDbContext> options) : base(options) { }

        public DbSet<User> User => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Dispute> Disputes => Set<Dispute>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Email).IsRequired().HasMaxLength(256);
                e.HasIndex(c => c.Email).IsUnique();
                e.Property(c => c.DisplayName).IsRequired().HasMaxLength(128);
                e.Property(c => c.PasswordHash).IsRequired();
                e.Property(c => c.Role).HasConversion<string>().HasMaxLength(32);
            });

            b.Entity<Account>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.AccountNumber).IsRequired().HasMaxLength(32);
                e.HasIndex(a => a.AccountNumber).IsUnique();
                e.HasOne<User>().WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Transaction>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Amount).HasPrecision(18, 2);
                e.Property(t => t.Currency).IsRequired().HasMaxLength(3);
                e.Property(t => t.MerchantName).IsRequired().HasMaxLength(128);
                e.Property(t => t.Description).HasMaxLength(256);
                e.Property(t => t.TransactionType).HasConversion<string>().HasMaxLength(16);
                e.HasOne<Account>().WithMany().HasForeignKey(t => t.AccountId).OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Dispute>(e =>
            {
                e.HasKey(d => d.Id);
                e.Property(d => d.Reason).IsRequired().HasMaxLength(1000);
                e.Property(d => d.Category).HasConversion<string>().HasMaxLength(32);
                e.Property(d => d.Status).HasConversion<string>().HasMaxLength(16);
                e.HasOne<Transaction>().WithMany().HasForeignKey(d => d.TransactionId).OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(d => d.TransactionId)
                 .IsUnique()
                 .HasFilter("\"Status\" IN ('Submitted', 'UnderReview')");

                e.HasMany(d => d.StatusHistory)
                 .WithOne()
                 .HasForeignKey(h => h.DisputeId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Metadata.FindNavigation(nameof(Dispute.StatusHistory))!
                 .SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            b.Entity<DisputeStatusHistory>(e =>
            {
                e.HasKey(h => h.Id);
                e.Property(h => h.FromStatus).HasConversion<string>().HasMaxLength(16);
                e.Property(h => h.ToStatus).HasConversion<string>().HasMaxLength(16);
                e.Property(h => h.Note).HasMaxLength(1000);
            });
        }
    }
}