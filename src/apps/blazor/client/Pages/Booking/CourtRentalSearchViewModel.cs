namespace FSH.Starter.Blazor.Client.Pages.Booking;

internal class CourtRentalSearchViewModel
{
    public DayOfWeek Weekday { get; set; } = DayOfWeek.Monday;
    public TimeOnly StartTime { get; set; } = new TimeOnly(8, 0);
    public int CourtId { get; set; } = 1;

    public List<CourtViewItem> Items { get; set; } = new();
        
}
