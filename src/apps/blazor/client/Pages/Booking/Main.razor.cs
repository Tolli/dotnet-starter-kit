using System.Globalization;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.NoonaApi;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using static MudBlazor.Icons.Custom;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class Main
{
    CompanyResponse Company = null;
    Customer Customer = null;
    ICollection<Event> Events = null;

    TenantDetail tenantDetails;

    public string? CustomerId;// = "YYlDdKN8KXDlpMB9s";

    public DateTime? StartDate = new DateTime(2025, 11, 03, 00, 00, 00, DateTimeKind.Local);
    public TimeSpan? StartTime = new TimeSpan(12, 00, 00);
    public DateTime? EndDate = new DateTime(2026, 05, 31, 23, 59, 59, DateTimeKind.Local);
    private List<Customer> _customers = new();

    protected EntityServerTableContext<CourtRentalResponse, Guid, CourtRentalViewModel> Context { get; set; } = default!;

    private EntityTable<CourtRentalResponse, Guid, CourtRentalViewModel> _table = default!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    public IEnumerable<System.Security.Claims.Claim>? Claims { get; set; }
    [Inject]
    protected INoonaApiClient _client { get; set; } = default!;

    [Inject]
    protected IApiClient _apiClient { get; set; } = default!;
    public string SearchValue { get; set; }
    public DateTime? DateValue { get; set; }

    public EventCallback<string> SearchStringChanged { get; set; }

    private IEnumerable<CourtRentalResponse> _results = Enumerable.Empty<CourtRentalResponse>();


    private async System.Threading.Tasks.Task<IEnumerable<CourtRentalResponse>> Search(string searchTerm, CancellationToken token)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            _results = Enumerable.Empty<CourtRentalResponse>();
            
        }
        else
        {
            var filter = new PaginationFilter() { PageSize = 10, Keyword = searchTerm };
            var courtrentalFilter = filter.Adapt<SearchCourtRentalsCommand>();
            var result = await _apiClient.SearchCourtRentalsEndpointAsync("1", courtrentalFilter);
            _results = result.Adapt<PaginationResponse<CourtRentalResponse>>().Items;
        }
        return _results;
    }


    private async System.Threading.Tasks.Task OnSearchStringChanged(string? searchTerm = null)
    {
        // await SearchStringChanged.InvokeAsync(SearchValue);
        if (string.IsNullOrEmpty(searchTerm))
        {
            _results = Enumerable.Empty<CourtRentalResponse>();
        }
        else
        {
            var filter = new PaginationFilter() { PageSize = 10, Keyword = searchTerm };
            var courtrentalFilter = filter.Adapt<SearchCourtRentalsCommand>();
            var result = await _apiClient.SearchCourtRentalsEndpointAsync("1", courtrentalFilter);
            _results = result.Adapt<PaginationResponse<CourtRentalResponse>>().Items;
        }
    }

    protected async override void OnInitialized()
    {
        try
        {
            var authState = await AuthState;
            Claims = authState.User.Claims;

            string tenantId = Claims.First(x => x.Type == "tenant").Value;

            tenantDetails = await _apiClient.GetTenantByIdEndpointAsync(tenantId);

            await LoadCustomersAsync();

            Context = new(
            entityName: "CourtRental",
            entityNamePlural: "CourtRentals",
            entityResource: FshResources.CourtRentals,
            fields: new()
            {
                new(courtrental => courtrental.Court, "Court", "Court"),
                new(courtrental => courtrental.Weekday, "Weekday", "Weekday"),
                new(courtrental => courtrental.StartTime.Substring(0,5), "StartTime", "Time"),
                new(courtrental => courtrental.StartDate.ToString("dd/MM/yyyy"), "Start Date", "Start Date"),
                new(courtrental => courtrental.StartDate.ToString("dd/MM/yyyy"), "End Date", "End Date"),
                new(courtrental => courtrental.Duration, "Duration", "Duration"),
                new(courtrental => courtrental.Amount, "Amount", "Amount"),
                new(courtrental => courtrental.Discount, "Discount", "Discount")
            },
            enableAdvancedSearch: true,
            idFunc: courtrental => courtrental.Id!.Value,
            searchFunc: async filter =>
            {
                var courtrentalFilter = filter.Adapt<SearchCourtRentalsCommand>();
                var result = await _apiClient.SearchCourtRentalsEndpointAsync("1", courtrentalFilter);
                return result.Adapt<PaginationResponse<CourtRentalResponse>>();
            },
            createFunc: async courtrental =>
            {
                await _apiClient.CreateCourtRentalEndpointAsync("1", courtrental.Adapt<CreateCourtRentalCommand>());
            },
            updateFunc: async (id, courtrental) =>
            {
                await _apiClient.UpdateCourtRentalEndpointAsync("1", id, courtrental.Adapt<UpdateCourtRentalCommand>());
            },
            deleteFunc: async id => await _apiClient.DeleteCourtRentalEndpointAsync("1", id));

            //GetCompany();
            //GetCustomer();


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


    }

    private async System.Threading.Tasks.Task LoadCustomersAsync()
    {
        if (_customers.Count == 0)
        {
            var response = await _client.ListCustomersAsync(tenantDetails.ExternalIdentifier, null, null, null, null, null, null);

            if (response.Count > 0)
            {
                _customers = response.ToList();
            }
        }
    }

    private async void CreateCustomer(string companyId)
    {
        var newCustomer = new Customer()
        {
            Name = "Öllarar",
            Email = "tolli@tolli.com",
            Company = companyId
        };

        //Customer = await _client.CreateCustomerAsync(newCustomer, null, null);
    }

    private async void GetCompany()
    {        

    }

    private async void CreateEvent(string[] courtResources, string customerId, string eventTypeId, string rrule, int duration, DateTime eventCreationDate, EventType eventType, string comments)
    {
        try
        {
            //string vollur1 = "HVsZ8UrAWZgGuHuT6d84wMF8";
            // vollur2
            
            foreach (var resource in courtResources)
            {
                //string filter =
                //"{\"from\":\"" + eventCreationDate.ToString("yyyy-MM-ddThh:mm:ss.fff+00:00", CultureInfo.InvariantCulture) + "\"," +
                //"\"to\":\"" + eventCreationDate.AddMinutes(50).ToString("yyyy-MM-ddThh:mm:ss.fff+00:00", CultureInfo.InvariantCulture) + "\"," +
                //"\"resources\":[\"" + resource + "\"]}";

                //Events = await _client.ListEventsAsync(CompanyId, null,null, filter, null, null, null, null, null, null, null);
                //if (Events.Count > 0)
                //{
                //    Console.WriteLine("Event already created");
                //    continue;
                //}
                //var resource = await _client.GetResourceAsync(vollur1, null, null);
                var resources = new ExpandableResources();
                resources.Add(resource);
                var eventTypes = new Infrastructure.NoonaApi.EventTypes();                
                eventTypes.Add(eventType);
                var myEvent = new Event()
                {
                    Number_of_guests = 1,
                    Starts_at = eventCreationDate,
                    Duration = duration,
                    Rrule = rrule,
                    Resources = resources,
                    Event_types = eventTypes,
                    Comment = comments
                };

                myEvent.AdditionalProperties.Add("company", tenantDetails.ExternalIdentifier);
                myEvent.AdditionalProperties.Add("customer", customerId);
                myEvent.AdditionalProperties.Add("event_type", eventTypeId);
                var returnedEvent = await _client.CreateEventAsync(null, myEvent, null, null);
            }

            
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
    }

    private async System.Threading.Tasks.Task SendEvent()
    {
        
        Customer = await _client.GetCustomerAsync(CustomerId, null, null);
        Company = await _client.GetCompanyAsync(tenantDetails.ExternalIdentifier, null, null);

        string eventTypeId = "bSCUocoS48KK2oxOK3ZGiMV6";
        string[] courtResources = new string[] { "P1zg1MNsBfIDVDWpV7GkRXjs", "HVsZ8UrAWZgGuHuT6d84wMF8" };
        var date = StartDate.Value.Add(StartTime.Value);

        string extractWeekDay = date.ToString("ddd", CultureInfo.InvariantCulture)
            .ToUpper(CultureInfo.InvariantCulture)[..2];
        int duration = 50;
        string rrule = $"FREQ=WEEKLY;INTERVAL=1;UNTIL={EndDate.Value.ToString("yyyyMMddThhmmssZ")};BYDAY={extractWeekDay}";
        var eventType = await _client.GetEventTypeAsync(eventTypeId, null, null);
        eventType.Duration = duration;
        string comments = "Greitt - Þórhallur Einisson\nÓgreitt - 50.000 - Hrund Guðmundsdóttir";
        CreateEvent(courtResources, CustomerId, eventTypeId, rrule, duration, date, eventType, comments);
    }

    private System.Threading.Tasks.Task OpenDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<MainDialog>(Company.Name, options);
    }

    private void ManageShares(in Guid? groupId) =>
        Navigation.NavigateTo($"/booking/courtrental/{groupId}/members");
}
