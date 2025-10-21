using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class Groups
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<GroupResponse, Guid, GroupViewModel> Context { get; set; } = default!;

    private EntityTable<GroupResponse, Guid, GroupViewModel> _table = default!;

    protected override void OnInitialized() =>
    Context = new(
            entityName: "Group",
            entityNamePlural: "Groups",
            entityResource: FshResources.Groups,
            fields: new()
            {
                new(group => group.Id, "Id", "Id"),
                new(group => group.Name, "Name", "Name"),
                new(group => group.StartDate, "StartDate", "StartDate", typeof(DateTime)),
                new(group => group.EndDate, "EndDate", "EndDate", typeof(DateTime))
            },
            enableAdvancedSearch: true,
            idFunc: group => group.Id!.Value,
            searchFunc: async filter =>
            {
                var groupFilter = filter.Adapt<SearchGroupsCommand>();
                var result = await _client.SearchGroupsEndpointAsync("1", groupFilter);
                return result.Adapt<PaginationResponse<GroupResponse>>();
            },
            createFunc: async group =>
            {
                await _client.CreateGroupEndpointAsync("1", group.Adapt<CreateGroupCommand>());
            },
            updateFunc: async (id, group) =>
            {
                await _client.UpdateGroupEndpointAsync("1", id, group.Adapt<UpdateGroupCommand>());
            },
            deleteFunc: async id => await _client.DeleteGroupEndpointAsync("1", id));
}

public class GroupViewModel : UpdateGroupCommand
{
}
