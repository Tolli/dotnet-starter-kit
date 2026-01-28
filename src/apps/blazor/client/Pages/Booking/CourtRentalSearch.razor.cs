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
    CourtRentalSearchViewModel _vm = new CourtRentalSearchViewModel();
    CourtRentalSearchViewModel _vm2 = new CourtRentalSearchViewModel();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_hasInitialLoad)
        {
            _hasInitialLoad = true;
            await UpdateCalendarItems();
            StateHasChanged();
        }
    }
    private async Task UpdateCalendarItems()
    {
        if (SelectionDate.HasValue)
        {
            await UpdateCalendar(SelectionDate.Value, _myCalendar1, SelectedCourt, _vm);
            await UpdateCalendar(SelectionDate.Value, _myCalendar2, SelectedCourt + 1, _vm2);
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
                                CourtRental = x.CourtRental,
                                Title = x.CourtRental.StartTime,
                                Start = x.StartDate,
                                End = x.EndDate,
                            }).ToList() ?? new List<CourtViewItem>();
            viewModel.Items.Clear();
            viewModel.Items.AddRange(addingItems);
            calendar.Refresh();
        }
    }
        
    private void CellClicked(DateTime args)
    {
        Console.WriteLine(args);
    }
    private void ItemClicked(CourtViewItem item)
    {
        Navigation.NavigateTo($"/booking/courtrental/{item.CourtRental.Id}/edit");
    }    
}
