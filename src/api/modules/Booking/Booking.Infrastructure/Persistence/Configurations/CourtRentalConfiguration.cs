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
        builder.Property(d => d.Weekday).HasConversion<string>();
        builder.HasOne(x => x.Group).WithMany(g => g.CourtRentals).HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.Weekday, x.StartTime, x.Court, x.StartDate, x.EndDate });
    }
}
