using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Persistence.Configurations;
internal sealed class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AmountTotal).HasPrecision(18, 0);
        builder.Property(x => x.AmountPaid).HasPrecision(18, 0);
        builder.Property(x => x.ExternalReference).HasMaxLength(100);
        builder.HasOne(x => x.GroupMember).WithMany(c => c.Receipts).HasForeignKey(x => x.GroupMemberId).OnDelete(DeleteBehavior.Cascade);
    }
}
