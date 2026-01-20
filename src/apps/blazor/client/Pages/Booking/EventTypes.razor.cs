using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.NoonaApi;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class EventTypes
{
    [Inject]
    protected INoonaApiClient _client { get; set; } = default!;

    [Inject]
    protected IApiClient _apiClient { get; set; } = default!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    public IEnumerable<System.Security.Claims.Claim>? Claims { get; set; }

    protected EntityServerTableContext<EventType, Guid, EventTypeViewModel> Context { get; set; } = default!;

    private EntityTable<EventType, Guid, EventTypeViewModel> _table = default!;

    TenantDetail tenantDetails;

    protected override void OnInitialized()
    {
        Context = new(
            entityName: "EventType",
            entityNamePlural: "EventTypes",
            entityResource: FshResources.ExternalSystems,
            fields: new()
            {
                new(eventtype => eventtype.Id, "Id", "Id"),
                new(eventtype => eventtype.Title, "Title", "Title")
            },
            enableAdvancedSearch: true,
            searchFunc: async filter =>
            {
                var authState = await AuthState;
                Claims = authState.User.Claims;

                string tenantId = Claims.First(x => x.Type == "tenant").Value;

                tenantDetails = await _apiClient.GetTenantByIdEndpointAsync(tenantId);

                var result = await _client.ListEventTypesAsync(tenantDetails.ExternalIdentifier, null, null, ["id", "title"], null);
                return new PaginationResponse<EventType>() { Items = [.. result], CurrentPage = 1, PageSize = result.Count, TotalCount = result.Count };
            });
    }
}

public class EventTypeViewModel : EventType
{
}
