using FSH.Starter.Blazor.Infrastructure.Api;
using Heron.MudCalendar;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public class CourtViewItem : CalendarItem
{
    public string Title { get; set; } = string.Empty;
    public CourtRentalResponse? CourtRental { get; set; }
    public Color Color { get; set; } = Color.Success;
    
}

public class CourtViewMember
{
    public string Name { get; set; } = string.Empty;
    public decimal AmountTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountOwed => AmountTotal - AmountPaid;
}
