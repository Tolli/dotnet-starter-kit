using System.ComponentModel.Design;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.NoonaApi;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class Resources
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    public IEnumerable<System.Security.Claims.Claim>? Claims { get; set; }

    [Inject]
    protected INoonaApiClient _client { get; set; } = default!;

    [Inject]
    protected IApiClient _apiClient { get; set; } = default!;

    TenantDetail tenantDetails;

    protected EntityServerTableContext<Resource, Guid, ResourceViewModel> Context { get; set; } = default!;

    private EntityTable<Resource, Guid, ResourceViewModel> _table = default!;

    protected override void OnInitialized()
    {
        Context = new(
            entityName: "Resource",
            entityNamePlural: "Resources",
            entityResource: FshResources.ExternalSystems,
            fields: new()
            {
                new(resource => resource.Id, "Id", "Id"),
                new(resource => resource.Name, "Name", "Name")
            },
            enableAdvancedSearch: true,
            searchFunc: async filter =>
            {
                var authState = await AuthState;
                Claims = authState.User.Claims;

                string tenantId = Claims.First(x => x.Type == "tenant").Value;

                tenantDetails = await _apiClient.GetTenantByIdEndpointAsync(tenantId);
                var result = await _client.ListResourcesAsync(tenantDetails.ExternalIdentifier, null, null, null, ["id", "name"], null, null);
                return new PaginationResponse<Resource>() { Items = [.. result], CurrentPage = 1, PageSize = result.Count, TotalCount = result.Count };
            });
    }
}

public class ResourceViewModel : Resource
{
}
