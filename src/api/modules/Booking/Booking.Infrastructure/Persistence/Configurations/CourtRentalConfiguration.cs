using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Persistence.Configurations;
internal sealed class CourtRentalConfiguration : IEntityTypeConfiguration<CourtRental>
{
    public void Configure(EntityTypeBuilder<CourtRental> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasOne(x => x.Group).WithMany(g => g.CourtRentals).HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.SetNull);
    }
}
