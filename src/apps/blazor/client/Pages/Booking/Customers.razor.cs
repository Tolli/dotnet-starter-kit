using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class Customers
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<CustomerResponse, Guid, CustomerViewModel> Context { get; set; } = default!;

    private EntityTable<CustomerResponse, Guid, CustomerViewModel> _table = default!;

    protected override void OnInitialized() =>
    Context = new(
            entityName: "Customer",
            entityNamePlural: "Customers",
            entityResource: FshResources.Customers,
            fields: new()
            {
                new(customer => customer.Id, "Id", "Id"),
                new(customer => customer.Name, "Name", "Name"),
                new(customer => customer.ClubNumber, "ClubNumber", "ClubNumber"),
                new(customer => customer.PhoneNumber, "PhoneNumber", "PhoneNumber"),
                new(customer => customer.Email, "Email", "Email"),
                new(customer => customer.Address, "Address", "Address"),
                new(customer => customer.PostalCode, "PostalCode", "PostalCode"),
                new(customer => customer.Ssn, "Ssn", "Ssn"),
                    new(customer => customer.CourtRentalShares, "Bookings", "Bookings", Template:CourtsTemplate),

            },
            enableAdvancedSearch: true,
            idFunc: customer => customer.Id!.Value,
            searchFunc: async filter =>
            {
                var customerFilter = filter.Adapt<SearchCustomersCommand>();
                var result = await _client.SearchCustomersEndpointAsync("1", customerFilter);
                return result.Adapt<PaginationResponse<CustomerResponse>>();
            },
            createFunc: async customer =>
            {
                await _client.CreateCustomerEndpointAsync("1", customer.Adapt<CreateCustomerCommand>());
            },
            updateFunc: async (id, customer) =>
            {
                await _client.UpdateCustomerEndpointAsync("1", id, customer.Adapt<UpdateCustomerCommand>());
            },
            deleteFunc: async id => await _client.DeleteCustomerEndpointAsync("1", id));

    private void ManageReceipts(in Guid? customerId) =>
        Navigation.NavigateTo($"/booking/customers/{customerId}/receipts");

}

public class CustomerViewModel : UpdateCustomerCommand
{
}
