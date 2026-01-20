using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Persistence.Configurations;
internal sealed class CourtRentalSessionConfiguration : IEntityTypeConfiguration<CourtRentalSession>
{
    public void Configure(EntityTypeBuilder<CourtRentalSession> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.CourtRental).WithMany(x => x.CourtRentalSessions).HasForeignKey(x => x.CourtRentalId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.Court, x.StartDate, x.EndDate });
    }
}
