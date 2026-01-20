using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Persistence.Configurations;
internal sealed class CourtRentalShareConfiguration : IEntityTypeConfiguration<CourtRentalShare>
{
    public void Configure(EntityTypeBuilder<CourtRentalShare> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AmountTotal).HasPrecision(18, 0);
        builder.Property(x => x.AmountPaid).HasPrecision(18, 0);
        builder.Property(x => x.ExternalReference).HasMaxLength(100);
        builder.HasOne(x => x.CourtRental).WithMany(c => c.CourtRentalShares).HasForeignKey(x => x.CourtRentalId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Customer).WithMany(c => c.CourtRentalShares).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
    }
}
