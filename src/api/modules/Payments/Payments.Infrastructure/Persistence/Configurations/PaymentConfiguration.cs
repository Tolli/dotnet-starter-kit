using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Payments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Payments.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CustomerId)
            .IsRequired();
        
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(x => x.Method)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(x => x.TransactionId)
            .HasMaxLength(100);
        
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        
        builder.Property(x => x.GatewayResponse)
            .HasMaxLength(2000);
        
        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);
        
        builder.Property(x => x.PaymentDate)
            .IsRequired();

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.InvoiceId);
        builder.HasIndex(x => x.TransactionId);
        builder.HasIndex(x => x.Status);
    }
}
