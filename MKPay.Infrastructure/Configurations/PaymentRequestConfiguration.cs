using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MKPay.Core.Entities;

namespace MKPay.Infrastructure.Configurations;

public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
{
    public void Configure(EntityTypeBuilder<PaymentRequest> builder)
    {
        builder.ToTable("PaymentRequests");

        builder.HasKey(pr => pr.Id);

        // Properties
        builder.Property(pr => pr.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(pr => pr.Currency)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(pr => pr.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(pr => pr.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(pr => pr.RequesterId);
        builder.HasIndex(pr => pr.RequesteeId);
        builder.HasIndex(pr => pr.Status);
        builder.HasIndex(pr => pr.ExpiresAt);

        // Relationships
        builder.HasOne(pr => pr.Requester)
            .WithMany(u => u.CreatedPaymentRequests)
            .HasForeignKey(pr => pr.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pr => pr.Requestee)
            .WithMany(u => u.ReceivedPaymentRequests)
            .HasForeignKey(pr => pr.RequesteeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}