using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Client.Pages.Booking;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.NoonaApi;
using FSH.Starter.Blazor.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Mapster;

namespace FSH.Starter.Blazor.Client.Layout;

public partial class MainLayout
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;
    [Parameter]
    public EventCallback<bool> OnDarkModeToggle { get; set; }
    [Parameter]
    public EventCallback<bool> OnRightToLeftToggle { get; set; }
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    private bool _drawerOpen;
    private bool _isDarkMode;
    
    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference preferences)
        {
            _drawerOpen = preferences.IsDrawerOpen;
            _isDarkMode = preferences.IsDarkMode;
        }
    }

    public async System.Threading.Tasks.Task ToggleDarkMode()
    {
        _isDarkMode = !_isDarkMode;
        await OnDarkModeToggle.InvokeAsync(_isDarkMode);
    }

    private async System.Threading.Tasks.Task DrawerToggle()
    {
        _drawerOpen = await ClientPreferences.ToggleDrawerAsync();
    }
    private void Logout()
    {
        var parameters = new DialogParameters
        {
                { nameof(Components.Dialogs.Logout.ContentText), "Do you want to logout from the system?"},
                { nameof(Components.Dialogs.Logout.ButtonText), "Logout"},
                { nameof(Components.Dialogs.Logout.Color), Color.Error}
            };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        DialogService.Show<Components.Dialogs.Logout>("Logout", parameters, options);
    }

    private void Profile()
    {
        Navigation.NavigateTo("/identity/account");
    }
}
