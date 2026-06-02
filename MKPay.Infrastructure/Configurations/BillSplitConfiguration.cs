using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MKPay.Core.Entities;

namespace MKPay.Infrastructure.Configurations;

public class BillSplitConfiguration : IEntityTypeConfiguration<BillSplit>
{
    public void Configure(EntityTypeBuilder<BillSplit> builder)
    {
        builder.ToTable("BillSplits");

        builder.HasKey(bs => bs.Id);

        // Properties
        builder.Property(bs => bs.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(bs => bs.Description)
            .HasMaxLength(1000);

        builder.Property(bs => bs.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(bs => bs.Currency)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        // Indexes
        builder.HasIndex(bs => bs.CreatorId);
        builder.HasIndex(bs => bs.IsSettled);
        builder.HasIndex(bs => bs.CreatedAt);

        // Relationships
        builder.HasOne(bs => bs.Creator)
            .WithMany()
            .HasForeignKey(bs => bs.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}