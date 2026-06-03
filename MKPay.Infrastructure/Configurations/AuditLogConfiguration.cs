using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MKPay.Core.Entities;

namespace MKPay.Infrastructure.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(al => al.Id);

        // Properties
        builder.Property(al => al.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(al => al.EntityType)
            .HasMaxLength(100);

        builder.Property(al => al.Details)
            .HasMaxLength(2000);

        builder.Property(al => al.IpAddress)
            .HasMaxLength(50);

        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(al => al.UserId);
        builder.HasIndex(al => al.CreatedAt);
        builder.HasIndex(al => al.Action);

        // Relationships
        builder.HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}