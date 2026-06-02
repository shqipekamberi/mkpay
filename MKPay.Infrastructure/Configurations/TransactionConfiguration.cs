using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MKPay.Core.Entities;

namespace MKPay.Infrastructure.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.Currency)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.ReferenceNumber)
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(t => t.ReferenceNumber)
            .IsUnique();

        builder.HasIndex(t => t.SenderAccountId);
        builder.HasIndex(t => t.ReceiverAccountId);
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.Status);

        // Relationships
        builder.HasOne(t => t.SenderAccount)
            .WithMany()
            .HasForeignKey(t => t.SenderAccountId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        builder.HasOne(t => t.ReceiverAccount)
            .WithMany()
            .HasForeignKey(t => t.ReceiverAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.BillSplit)
            .WithMany(bs => bs.Transactions)
            .HasForeignKey(t => t.BillSplitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.PaymentRequest)
            .WithMany(pr => pr.Transactions)
            .HasForeignKey(t => t.PaymentRequestId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}