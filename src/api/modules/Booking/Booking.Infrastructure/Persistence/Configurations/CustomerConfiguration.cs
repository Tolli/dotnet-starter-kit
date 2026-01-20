using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Persistence.Configurations;
internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{ 
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.Ssn).HasMaxLength(20);
        builder.Property(x => x.PostalCode).HasMaxLength(3);
        builder.Property(x => x.ClubNumber).HasMaxLength(20);
        builder.Property(builder => builder.Address).HasMaxLength(200);
        builder.Property(builder => builder.Email).HasMaxLength(100);
        builder.Property(builder => builder.PhoneNumber).HasMaxLength(20);
    }
}
