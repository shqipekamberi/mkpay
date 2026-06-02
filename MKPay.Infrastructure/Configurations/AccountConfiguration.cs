using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MKPay.Core.Entities;

namespace MKPay.Infrastructure.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        // Table name
        builder.ToTable("Accounts");

        // Primary key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.AccountNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Currency)
            .HasConversion<string>() // Store enum as string
            .HasMaxLength(10)
            .IsRequired();

        // Indexes
        builder.HasIndex(a => a.UserId)
            .IsUnique(); // One account per user

        builder.HasIndex(a => a.AccountNumber)
            .IsUnique();

        // Relationships
        builder.HasOne(a => a.User)
            .WithOne(u => u.Account)
            .HasForeignKey<Account>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}