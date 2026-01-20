using Finbuckle.MultiTenant.Abstractions;

namespace FSH.Framework.Infrastructure.Tenant.Abstractions;
public interface IFshTenantInfo : ITenantInfo
{
    string? ExternalIdentifier { get; set; }
    string? ConnectionString { get; set; }
}
