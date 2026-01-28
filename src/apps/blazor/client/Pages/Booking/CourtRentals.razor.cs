using System.Globalization;
using System.Reflection.Emit;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using static FSH.Starter.Blazor.Client.Pages.Identity.Users.Audit;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class CourtRentals
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;
    private DayOfWeek? _selectedDayOfWeek;
    protected DayOfWeek? SelectedDayOfWeek
    {
        get { return _selectedDayOfWeek; }
        set { 
            if (_selectedDayOfWeek != value)
            {
                _selectedDayOfWeek = value;
                _table.ReloadDataAsync();
            }
        }
    }
    private int? _selectedCourt;
    protected int? SelectedCourt
    {
        get
        {
            return _selectedCourt;
        }
        set
        {
            if (_selectedCourt != value)
            {
                _selectedCourt = value;
                _table.ReloadDataAsync();
            }
        }
    }

    private TimeOnly _startTime = new TimeOnly(08,00);
    protected TimeOnly StartTime
    {
        get
        {
            return _startTime;
        }
        set
        {
            if(_startTime != value)
            {
                _startTime = value;
                _table.ReloadDataAsync();
            }
        }
    }
    protected EntityServerTableContext<CourtRentalResponse, Guid, CourtRentalViewModel> Context { get; set; } = default!;

    private EntityTable<CourtRentalResponse, Guid, CourtRentalViewModel> _table = default!;
    

    private void ManageShares(in Guid? groupId) =>
        Navigation.NavigateTo($"/booking/courtrental/{groupId}/members");
    protected override void OnInitialized() =>
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
                new(courtrental => (courtrental.CourtRentalShares?.Sum(x => x.AmountTotal) ?? 0), "Amount Total", "Amount Total"),
                new(courtrental => (courtrental.CourtRentalShares?.Sum(x => x.AmountPaid) ?? 0), "Amount Paid", "Amount Paid"),
                new(courtrental => (courtrental.CourtRentalShares?.Sum(x => x.AmountTotal - x.AmountPaid) ?? 0), "Amount Owed", "Amount Owed"),
                new(courtrental => courtrental.Discount, "Discount", "Discount"),
                new(courtrental => courtrental.CourtRentalShares, "Members", "Members", Template: PlayersTemplate)
            },
            enableAdvancedSearch: false,
            idFunc: courtrental => courtrental.Id!.Value,
            hasExtraActionsFunc: () => true,
            searchFunc: async filter =>
            {
                var courtrentalFilter = filter.Adapt<SearchCourtRentalsCommand>();
                courtrentalFilter.Weekday = SelectedDayOfWeek.ToString();
                courtrentalFilter.Court = SelectedCourt;
                courtrentalFilter.FromTime = StartTime.ToString("HH:mm");
                var result = await _client.SearchCourtRentalsEndpointAsync("1", courtrentalFilter);
                return result.Adapt<PaginationResponse<CourtRentalResponse>>();
            },
            createFunc: async courtrental =>
            {
                await _client.CreateCourtRentalEndpointAsync("1", courtrental.Adapt<CreateCourtRentalCommand>());
            },
            updateFunc: async (id, courtrental) =>
            {
                await _client.UpdateCourtRentalEndpointAsync("1", id, courtrental.Adapt<UpdateCourtRentalCommand>());
            },
            deleteFunc: async id => await _client.DeleteCourtRentalEndpointAsync("1", id));
}

public class CourtRentalViewModel : UpdateCourtRentalCommand
{
    private TimeSpan? _startTime;
    public TimeSpan? StartTimeEditor
    {
        get {
            if (!_startTime.HasValue && !String.IsNullOrEmpty(StartTime)) _startTime = TimeSpan.ParseExact(StartTime, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);
            return _startTime;
        } 
        set
        {
            _startTime = value;
            StartTime = _startTime?.ToString("hh\\:mm\\:ss") ?? "00:00:00";
        }
    }
}
