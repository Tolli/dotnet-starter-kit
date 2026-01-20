using Heron.MudCalendar;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public class CourtViewItem : CalendarItem
{
    public Guid CourtRentalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public IList<string> Members { get; set; }
    public Color Color { get; set; } = Color.Success;
    
}
