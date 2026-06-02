using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MKPay.Core.Entities;

namespace MKPay.Infrastructure.Configurations;

public class BillSplitParticipantConfiguration : IEntityTypeConfiguration<BillSplitParticipant>
{
    public void Configure(EntityTypeBuilder<BillSplitParticipant> builder)
    {
        builder.ToTable("BillSplitParticipants");

        builder.HasKey(bsp => bsp.Id);

        // Properties
        builder.Property(bsp => bsp.AmountOwed)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Indexes
        builder.HasIndex(bsp => new { bsp.BillSplitId, bsp.UserId })
            .IsUnique(); // User can only participate once per bill split

        builder.HasIndex(bsp => bsp.IsPaid);

        // Relationships
        builder.HasOne(bsp => bsp.BillSplit)
            .WithMany(bs => bs.Participants)
            .HasForeignKey(bsp => bsp.BillSplitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bsp => bsp.User)
            .WithMany()
            .HasForeignKey(bsp => bsp.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}