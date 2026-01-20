using System.Globalization;
using System.Reflection.Emit;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Heron.MudCalendar;
using Mapster;
using Microsoft.AspNetCore.Components;
using static FSH.Starter.Blazor.Client.Pages.Identity.Users.Audit;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class CourtRentalSearch
{
    [Inject]
    protected IApiClient Client { get; set; } = default!;

    private DateTime? _selectionDate = DateTime.Now.Date;
    private int _selectedCourt = 1;
    private TimeOnly _dayStartTime = new TimeOnly(8, 0);
    private bool _hasInitialLoad;
    private DateTime _currentDay = DateTime.Now.Date;
    
    public DateTime? SelectionDate
    {
        get => _selectionDate;
        set
        {
            if (_selectionDate != value)
            {
                _selectionDate = value;
                if (value.HasValue)
                {
                    _currentDay = value.Value;
                }
                InvokeAsync(async () => await UpdateCalendarItems());
            }
        }
    }

    public int SelectedCourt
    {
        get => _selectedCourt;
        set
        {
            if (_selectedCourt != value)
            {
                _selectedCourt = value;
                InvokeAsync(async () => await UpdateCalendarItems());
            }
        }
    }

    //protected EntityServerTableContext<CourtRentalResponse, Guid, CourtRentalViewModel> Context { get; set; } = default!;

    //private ModifiedEntityTable<CourtRentalResponse, Guid, CourtRentalViewModel> _table = default!;

    private MudCalendar<CourtViewItem> _myCalendar1 = default!;
    private MudCalendar<CourtViewItem> _myCalendar2 = default!;
    private MudCalendar<CourtViewItem> _myCalendar3 = default!;
    private MudCalendar<CourtViewItem> _myCalendar4 = default!;
    private MudCalendar<CourtViewItem> _myCalendar5 = default!;
    CourtRentalSearchViewModel vm = new CourtRentalSearchViewModel();
    CourtRentalSearchViewModel vm2 = new CourtRentalSearchViewModel();
    CourtRentalSearchViewModel vm3 = new CourtRentalSearchViewModel();
    CourtRentalSearchViewModel vm4 = new CourtRentalSearchViewModel();
    CourtRentalSearchViewModel vm5 = new CourtRentalSearchViewModel();

    //private void ManageShares(in Guid? groupId) =>
    //    Navigation.NavigateTo($"/booking/courtrental/{groupId}/members");

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_hasInitialLoad)
        {
            _hasInitialLoad = true;
            await UpdateCalendarItems();
            StateHasChanged();
        }
    }

    //protected override void OnInitialized() =>
    //Context = new(
    //        entityName: "CourtRental",
    //        entityNamePlural: "CourtRentals",
    //        entityResource: FshResources.CourtRentals,
    //        fields: new()
    //        {
    //            new(courtrental => courtrental.Court, "Court", "Court"),
    //            new(courtrental => courtrental.Weekday, "Weekday", "Weekday"),
    //            new(courtrental => courtrental.StartTime.Substring(0,5), "StartTime", "Time", Template: EditIdTemplate),
    //            new(courtrental => courtrental.StartDate.ToString("dd/MM/yyyy"), "Start Date", "Start Date"),
    //            new(courtrental => courtrental.EndDate.ToString("dd/MM/yyyy"), "End Date", "End Date"),
    //            new(courtrental => courtrental.Duration, "Duration", "Duration"),
    //            new(courtrental => courtrental.CourtRentalShares, "Members", "Members", Template:PlayersTemplate),
    //            new(courtrental => courtrental.CourtRentalShares?.Sum(x => x.AmountTotal) ?? 0, "Amount Total", "Amount Total"),
    //            // new(courtrental => courtrental.CourtRentalShares.Sum(x => x.AmountPaid), "Amount Paid", "Amount Paid"),
    //            new(courtrental => courtrental.CourtRentalShares?.Sum(x => x.AmountTotal - x.AmountPaid) ?? 0, "Amount Owed", "Amount Owed"),
    //            new(courtrental => courtrental.Discount, "Discount", "Discount")
    //        },
    //        enableAdvancedSearch: true,
    //        idFunc: courtrental => courtrental.Id!.Value,
    //        hasExtraActionsFunc: () => true,
    //        searchFunc: async filter =>
    //        {
    //            var courtrentalFilter = filter.Adapt<SearchCourtRentalsByHouseDayTimeCourtCommand>();
    //            courtrentalFilter = ParseFilter(courtrentalFilter);
    //            //if (courtrentalFilter != null)
    //            //{


    //                // Defensive: Ensure Items is not set directly (BL0005). Instead, use a method/property or event to update calendar items.
    //                // Defensive: Use null-conditional and null-coalescing operators for CS8604.
    //                // Defensive: Use CultureInfo.InvariantCulture for S6580.

    //                // Instead of setting Items directly, call a method to update the calendar (BL0005)
    //                //UpdateCalendarItems(_myCalendar1, result);

    //                //return result.Adapt<PaginationResponse<CourtRentalResponse>>();
    //            //}
    //            return new PaginationResponse<CourtRentalResponse>();
    //        },
    //        createFunc: async courtrental =>
    //        {
    //            await Client.CreateCourtRentalEndpointAsync("1", courtrental.Adapt<CreateCourtRentalCommand>());
    //        },
    //        updateFunc: async (id, courtrental) =>
    //        {
    //            await Client.UpdateCourtRentalEndpointAsync("1", id, courtrental.Adapt<UpdateCourtRentalCommand>());
    //        },
    //        deleteFunc: async id => await Client.DeleteCourtRentalEndpointAsync("1", id));


    // Helper method to update calendar items (avoids BL0005)
    private async Task UpdateCalendarItems()
    {
        if (SelectionDate.HasValue)
        {
            await UpdateCalendar(SelectionDate.Value, _myCalendar1, SelectedCourt, vm);
            await UpdateCalendar(SelectionDate.Value, _myCalendar2, SelectedCourt + 1, vm2);
        }
    }

    private async Task UpdateCalendar(DateTime date, MudCalendar<CourtViewItem> calendar, int court, CourtRentalSearchViewModel viewModel)
    {
        var filter = new SearchCourtRentalSessionsByDateCourtCommand
        {
            CourtFrom = court,
            CourtTo = court,
            PageSize = 1000,
            StartDate = date.Date,
            EndDate = date.Date.AddHours(23).AddMinutes(59).AddSeconds(59)
        };
        
        var result = await Client.SearchCourtRentalSessionsByDateCourtEndpointAsync("1", filter);

        if (calendar != null)
        {
            var addingItems = result?.Items?
                            .Select(x => new CourtViewItem()
                            {
                                CourtRentalId = x.CourtRental.Id!.Value,
                                Text = string.Join(", ", x.CourtRental.CourtRentalShares?.Select(s => s.Customer?.Name ?? string.Empty) ?? Enumerable.Empty<string>()),
                                Title = x.CourtRental.StartTime,
                                Members = x.CourtRental.CourtRentalShares?.Select(x => x.Customer?.Name ?? string.Empty).ToList() ?? new List<string>(),
                                Start = x.StartDate,
                                End = x.EndDate,
                            }).ToList() ?? new List<CourtViewItem>();
            viewModel.Items.Clear();
            viewModel.Items.AddRange(addingItems);
            calendar.ScrollToTime(new TimeOnly(DateTime.Now.Ticks));
            calendar.Refresh();
        }
    }

    //private static SearchCourtRentalsByHouseDayTimeCourtCommand ParseFilter(SearchCourtRentalsByHouseDayTimeCourtCommand filter)
    //{
    //    if (filter.Keyword?.Length == 0)
    //        return null;

    //    if (filter.Keyword?.Length >= 9 && filter.Keyword?.Length <= 11)
    //    {
    //        if (!(filter.Keyword.Substring(0, 1) is "1" or "2"))
    //            return null;

    //        Dictionary<string, string> days = new Dictionary<string, string> { { "MAN", "Monday" }, { "ÞRI", "Tuesday" }, { "MID", "Wednesday" }, { "FIM", "Thursday" }, { "FOS", "Friday" }, { "LAU", "Saturday" }, { "SUN", "Sunday" } };

    //        string day = filter.Keyword.Substring(1, 3).ToUpper().Replace("Á", "A").Replace("Ð", "D");
    //        var replacement = days.FirstOrDefault(x => x.Key == day);
    //        if (replacement.Key != day)
    //        {
    //            return null;
    //        }

    //        var time = filter.Keyword.Substring(4, 5).Replace(".", ":");

    //        int? court = null;
    //        if (filter.Keyword.Length > 9)
    //            if (int.TryParse(filter.Keyword[9..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int number))
    //            {
    //                court = number;
    //            }

    //        filter.House = int.Parse(filter.Keyword.Substring(0, 1), CultureInfo.InvariantCulture);
    //        filter.Weekday = replacement.Value;
    //        filter.StartTime = time;
    //        filter.StartDate = new DateTime(2025, 09, 01);
    //        filter.EndDate = new DateTime(2026, 05, 31, 23, 59, 59);
    //        if (court.HasValue)
    //        {
    //            filter.Court = court;
    //        }
    //        filter.Keyword = "";

    //        return filter;
    //    }

    //    return null;
    //}

    //private async Task CourtChanged(int newCourtId)
    //{
    //    await UpdateCalendarItems();
    //}
    //private async Task TimeChanged(TimeOnly timeOnly)
    //{
    //    await UpdateCalendarItems();
    //}
    //private async Task DayChanged(DayOfWeek dayOfWeek)
    //{
    //    await UpdateCalendarItems();
    //}
    private void CellClicked(DateTime args)
    {
        Console.WriteLine(args);
    }
    private void ItemClicked(CourtViewItem item)
    {
        Console.WriteLine(item);
    }    
    //private async Task DateSelectionChanged(DateTime? args)
    //{
    //    SelectionDate = args;
    //}
    //private async Task CourtSelected(int args)
    //{
    //    SelectedCourt = args;
    //}
}
