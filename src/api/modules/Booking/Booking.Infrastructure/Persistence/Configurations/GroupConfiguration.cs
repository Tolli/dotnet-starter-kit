using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Persistence.Configurations;
internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.StartDate);
        builder.Property(x => x.EndDate);
        builder.HasMany(x => x.CourtRentals).WithOne(x => x.Group).HasForeignKey(cr => cr.GroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Members).WithOne(x => x.Group).HasForeignKey(gm => gm.GroupId).OnDelete(DeleteBehavior.Cascade);
    }
}
