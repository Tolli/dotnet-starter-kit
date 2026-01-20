using System.Collections.ObjectModel;

namespace FSH.Starter.Shared.Authorization;

public static class FshPermissions
{
    private static readonly FshPermission[] AllPermissions =
    [     
        //tenants
        new("View Tenants", FshActions.View, FshResources.Tenants, IsRoot: true),
        new("Create Tenants", FshActions.Create, FshResources.Tenants, IsRoot: true),
        new("Update Tenants", FshActions.Update, FshResources.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", FshActions.UpgradeSubscription, FshResources.Tenants, IsRoot: true),

        //identity
        new("View Users", FshActions.View, FshResources.Users),
        new("Search Users", FshActions.Search, FshResources.Users),
        new("Create Users", FshActions.Create, FshResources.Users),
        new("Update Users", FshActions.Update, FshResources.Users),
        new("Delete Users", FshActions.Delete, FshResources.Users),
        new("Export Users", FshActions.Export, FshResources.Users),
        new("View UserRoles", FshActions.View, FshResources.UserRoles),
        new("Update UserRoles", FshActions.Update, FshResources.UserRoles),
        new("View Roles", FshActions.View, FshResources.Roles),
        new("Create Roles", FshActions.Create, FshResources.Roles),
        new("Update Roles", FshActions.Update, FshResources.Roles),
        new("Delete Roles", FshActions.Delete, FshResources.Roles),
        new("View RoleClaims", FshActions.View, FshResources.RoleClaims),
        new("Update RoleClaims", FshActions.Update, FshResources.RoleClaims),
        
        //products
        new("View Products", FshActions.View, FshResources.Products, IsBasic: true),
        new("Search Products", FshActions.Search, FshResources.Products, IsBasic: true),
        new("Create Products", FshActions.Create, FshResources.Products),
        new("Update Products", FshActions.Update, FshResources.Products),
        new("Delete Products", FshActions.Delete, FshResources.Products),
        new("Export Products", FshActions.Export, FshResources.Products),

        //brands
        new("View Brands", FshActions.View, FshResources.Brands, IsBasic: true),
        new("Search Brands", FshActions.Search, FshResources.Brands, IsBasic: true),
        new("Create Brands", FshActions.Create, FshResources.Brands),
        new("Update Brands", FshActions.Update, FshResources.Brands),
        new("Delete Brands", FshActions.Delete, FshResources.Brands),
        new("Export Brands", FshActions.Export, FshResources.Brands),

        //customers
        new("View Customers", FshActions.View, FshResources.Customers, IsBasic: true),
        new("Search Customers", FshActions.Search, FshResources.Customers, IsBasic: true),
        new("Create Customers", FshActions.Create, FshResources.Customers),
        new("Update Customers", FshActions.Update, FshResources.Customers),
        new("Delete Customers", FshActions.Delete, FshResources.Customers),
        new("Export Customers", FshActions.Export, FshResources.Customers),

        //courtrentals
        new("View CourtRentals", FshActions.View, FshResources.CourtRentals, IsBasic: true),
        new("Search CourtRentals", FshActions.Search, FshResources.CourtRentals, IsBasic: true),
        new("Create CourtRentals", FshActions.Create, FshResources.CourtRentals),
        new("Update CourtRentals", FshActions.Update, FshResources.CourtRentals),
        new("Delete CourtRentals", FshActions.Delete, FshResources.CourtRentals),
        new("Export CourtRentals", FshActions.Export, FshResources.CourtRentals),

        //courtrentalshares
        new("View CourtRentalShares", FshActions.View, FshResources.CourtRentalShares, IsBasic: true),
        new("Search CourtRentalShares", FshActions.Search, FshResources.CourtRentalShares, IsBasic: true),
        new("Create CourtRentalShares", FshActions.Create, FshResources.CourtRentalShares),
        new("Update CourtRentalShares", FshActions.Update, FshResources.CourtRentalShares),
        new("Delete CourtRentalShares", FshActions.Delete, FshResources.CourtRentalShares),
        new("Export CourtRentalShares", FshActions.Export, FshResources.CourtRentalShares),

        //receipts
        new("View Receipts", FshActions.View, FshResources.Receipts, IsBasic: true),
        new("Search Receipts", FshActions.Search, FshResources.Receipts, IsBasic: true),
        new("Create Receipts", FshActions.Create, FshResources.Receipts),
        new("Update Receipts", FshActions.Update, FshResources.Receipts),
        new("Delete Receipts", FshActions.Delete, FshResources.Receipts),
        new("Export Receipts", FshActions.Export, FshResources.Receipts),

        //groups
        new("View Groups", FshActions.View, FshResources.Groups, IsBasic: true),
        new("Search Groups", FshActions.Search, FshResources.Groups, IsBasic: true),
        new("Create Groups", FshActions.Create, FshResources.Groups),
        new("Update Groups", FshActions.Update, FshResources.Groups),
        new("Delete Groups", FshActions.Delete, FshResources.Groups),
        new("Export Groups", FshActions.Export, FshResources.Groups),

        //groupmembers
        new("View GroupMembers", FshActions.View, FshResources.GroupMembers, IsBasic: true),
        new("Search GroupMembers", FshActions.Search, FshResources.GroupMembers, IsBasic: true),
        new("Create GroupMembers", FshActions.Create, FshResources.GroupMembers),
        new("Update GroupMembers", FshActions.Update, FshResources.GroupMembers),
        new("Delete GroupMembers", FshActions.Delete, FshResources.GroupMembers),
        new("Export GroupMembers", FshActions.Export, FshResources.GroupMembers),

        //externalsystems
        new("View ExternalSystems", FshActions.View, FshResources.ExternalSystems, IsBasic: true),
        new("Search ExternalSystems", FshActions.Search, FshResources.ExternalSystems, IsBasic: true),
        new("Create ExternalSystems", FshActions.Create, FshResources.ExternalSystems),
        new("Update ExternalSystems", FshActions.Update, FshResources.ExternalSystems),
        new("Delete ExternalSystems", FshActions.Delete, FshResources.ExternalSystems),
        new("Export ExternalSystems", FshActions.Export, FshResources.ExternalSystems),

        //todos
        new("View Todos", FshActions.View, FshResources.Todos, IsBasic: true),
        new("Search Todos", FshActions.Search, FshResources.Todos, IsBasic: true),
        new("Create Todos", FshActions.Create, FshResources.Todos),
        new("Update Todos", FshActions.Update, FshResources.Todos),
        new("Delete Todos", FshActions.Delete, FshResources.Todos),
        new("Export Todos", FshActions.Export, FshResources.Todos),

        new("View Hangfire", FshActions.View, FshResources.Hangfire),
        new("View Dashboard", FshActions.View, FshResources.Dashboard),



        //audit
        new("View Audit Trails", FshActions.View, FshResources.AuditTrails),
    ];

    public static IReadOnlyList<FshPermission> All { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions);
    public static IReadOnlyList<FshPermission> Root { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Admin { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<FshPermission> Basic { get; } = new ReadOnlyCollection<FshPermission>(AllPermissions.Where(p => p.IsBasic).ToArray());
}

public record FshPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource)
    {
        return $"Permissions.{resource}.{action}";
    }
}


