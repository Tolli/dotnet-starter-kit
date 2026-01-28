using System;
using FluentValidation;
using FSH.Starter.Blazor.Client.Components;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Infrastructure.Auth;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Booking;

public partial class CourtRentalEdit
{
    [Parameter]
    public string? Id { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    protected IApiClient ApiClient { get; set; } = default!;

    private MudForm _form = default!;
    private CourtRentalEditModel _model = new();
    private CourtRentalEditModelValidator _modelValidator = new();
    
    private List<CourtRentalShareEditModel> _shares = new();
    private List<CourtRentalShareEditModel> _originalShares = new();
    
    private TimeSpan? _startTime;
    private DateTime? _startDate;
    private DateTime? _endDate;

    private string _title = "Edit Court Rental";
    private string _description = string.Empty;
    
    private bool _loaded;
    private bool _canEdit;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEdit = await AuthService.HasPermissionAsync(state.User, FshActions.Update, FshResources.CourtRentals);

        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Navigation.NavigateTo("/booking/courtrentalsearch");
            return;
        }

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetCourtRentalEndpointAsync("1", new Guid(Id)), Toast, Navigation)
            is CourtRentalResponse courtRental)
        {
            _model = new CourtRentalEditModel
            {
                Id = courtRental.Id!.Value,
                Court = courtRental.Court,
                Weekday = courtRental.Weekday,
                StartTime = courtRental.StartTime,
                Duration = courtRental.Duration,
                Amount = courtRental.Amount,
                Discount = courtRental.Discount
            };

            _description = $"Court {courtRental.Court} - {courtRental.Weekday} {courtRental.StartTime}";

            // Parse start time
            if (TimeOnly.TryParse(courtRental.StartTime, out var timeOnly))
            {
                _startTime = new TimeSpan(timeOnly.Hour, timeOnly.Minute, 0);
            }

            // Parse dates
            _startDate = courtRental.StartDate;
            _endDate = courtRental.EndDate;

            // Load shares
            await LoadSharesAsync();
        }

        _loaded = true;
        StateHasChanged(); // Add this line to trigger re-render
    }

    private async Task LoadSharesAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.GetCourtRentalSharesByCourtRentalIdEndpointAsync("1", new Guid(Id!),
                    new GetCourtRentalSharesByCourtRentalIdCommand { CourtRentalId = new Guid(Id!) }),
                Toast, Navigation)
            is CourtRentalShareResponsePagedList response)
        {
            _shares = response.Items.Select(share => new CourtRentalShareEditModel
            {
                Id = share.Id,
                CourtRentalId = share.CourtRentalId!.Value,
                Customer = share.Customer,
                AmountTotal = share.AmountTotal,
                AmountPaid = share.AmountPaid,
                IsEditing = false,
                IsNew = false
            }).ToList();

            // Keep a copy of original data for cancel operations
            _originalShares = _shares.Select(s => s.Clone()).ToList();
        }
    }

    private async Task SaveCourtRentalAsync()
    {
        await _form.Validate();
        if (!_form.IsValid)
            return;

        // Update model with date/time values
        if (_startTime.HasValue)
        {
            _model.StartTime = new TimeOnly(_startTime.Value.Hours, _startTime.Value.Minutes).ToString("HH:mm");
        }

        var command = new UpdateCourtRentalCommand
        {
            Id = _model.Id,
            Court = _model.Court,
            Weekday = _model.Weekday,
            StartTime = _model.StartTime,
            StartDate = _startDate ?? DateTime.Now,
            EndDate = _endDate ?? DateTime.Now.AddMonths(6),
            Duration = _model.Duration,
            Amount = _model.Amount,
            Discount = _model.Discount
        };

        await ApiHelper.ExecuteCallGuardedAsync(
            () => ApiClient.UpdateCourtRentalEndpointAsync("1", _model.Id, command),
            Toast,
            successMessage: "Court rental updated successfully.");
    }

    private void AddNewShare()
    {
        var newShare = new CourtRentalShareEditModel
        {
            CourtRentalId = new Guid(Id!),
            Customer = new CustomerResponse(),
            AmountTotal = 0,
            AmountPaid = 0,
            IsEditing = true,
            IsNew = true
        };
        _shares.Add(newShare);
    }

    private void EditShare(CourtRentalShareEditModel share)
    {
        // Store original values
        var index = _shares.IndexOf(share);
        if (index >= 0)
        {
            _originalShares[index] = share.Clone();
        }
        share.IsEditing = true;
    }

    private void CancelEditShare(CourtRentalShareEditModel share)
    {
        if (share.IsNew)
        {
            _shares.Remove(share);
        }
        else
        {
            // Restore original values
            var index = _shares.IndexOf(share);
            if (index >= 0 && index < _originalShares.Count)
            {
                var original = _originalShares[index];
                share.Customer = original.Customer;
                share.AmountTotal = original.AmountTotal;
                share.AmountPaid = original.AmountPaid;
                share.IsEditing = false;
            }
        }
    }

    private async Task SaveShareAsync(CourtRentalShareEditModel share)
    {
        if (share.Customer?.Id == null)
        {
            Toast.Add("Please select a customer", MudBlazor.Severity.Warning);
            return;
        }

        if (share.IsNew)
        {
            var command = new CreateCourtRentalShareCommand
            {
                CourtRentalId = new Guid(Id!),
                CustomerId = share.Customer.Id.Value,
                AmountTotal = share.AmountTotal,
                AmountPaid = share.AmountPaid
            };

            if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.CreateCourtRentalShareEndpointAsync("1", command),
                Toast,
                Navigation) is CreateCourtRentalShareResponse createResult)
            {
                share.Id = createResult.Id;
                share.IsNew = false;
                share.IsEditing = false;
            }            
        }
        else if (share.Id.HasValue)
        {
            var command = new UpdateCourtRentalShareCommand
            {
                Id = share.Id.Value,
                CourtRentalId = new Guid(Id!),
                CustomerId = share.Customer.Id!.Value,
                AmountTotal = share.AmountTotal,
                AmountPaid = share.AmountPaid
            };

            await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.UpdateCourtRentalShareEndpointAsync("1", share.Id.Value, command),
                Toast,
                successMessage: "Share updated successfully.");

            share.IsEditing = false;
            StateHasChanged(); // Trigger re-render after save
        }
    }

    private async Task DeleteShareAsync(CourtRentalShareEditModel share)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Delete",
            "Are you sure you want to delete this share?",
            yesText: "Delete",
            cancelText: "Cancel");

        if (result == true)
        {
            if (share.Id.HasValue)
            {
                await ApiHelper.ExecuteCallGuardedAsync(
                    () => ApiClient.DeleteCourtRentalShareEndpointAsync("1", share.Id.Value),
                    Toast,
                    successMessage: "Share deleted successfully.");
            }

            _shares.Remove(share);
            StateHasChanged(); // Trigger re-render after delete
        }
    }

    private async Task<IEnumerable<CustomerResponse>> SearchCustomer(string value, CancellationToken token)
    {
        var result = new List<CustomerResponse>();
        
        if (string.IsNullOrEmpty(value) || value.Length < 3)
            return result;

        var request = new SearchCustomersCommand
        {
            PageSize = 10,
            Keyword = value
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.SearchCustomersEndpointAsync("1", request), Toast, Navigation)
            is CustomerResponsePagedList response)
        {
            result = response.Items.ToList();
        }

        return result;
    }
}

// Edit Models
public class CourtRentalEditModel
{
    public Guid Id { get; set; }
    public int Court { get; set; } = 1;
    public string Weekday { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public int Duration { get; set; }
    public double Amount { get; set; }
    public double Discount { get; set; }
}

public class CourtRentalShareEditModel
{
    public Guid? Id { get; set; }
    public Guid CourtRentalId { get; set; }
    public CustomerResponse Customer { get; set; } = new();
    public double AmountTotal { get; set; }
    public double AmountPaid { get; set; }
    public bool IsEditing { get; set; }
    public bool IsNew { get; set; }

    public CourtRentalShareEditModel Clone()
    {
        return new CourtRentalShareEditModel
        {
            Id = this.Id,
            CourtRentalId = this.CourtRentalId,
            Customer = this.Customer,
            AmountTotal = this.AmountTotal,
            AmountPaid = this.AmountPaid,
            IsEditing = this.IsEditing,
            IsNew = this.IsNew
        };
    }
}

public class CourtRentalEditModelValidator : AbstractValidator<CourtRentalEditModel>
{
    public CourtRentalEditModelValidator()
    {
        RuleFor(x => x.Court)
            .NotEmpty().WithMessage("Court is required");

        RuleFor(x => x.Weekday)
            .NotEmpty().WithMessage("Weekday is required");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<CourtRentalEditModel>.CreateWithOptions(
            (CourtRentalEditModel)model,
            x => x.IncludeProperties(propertyName)));
        
        return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
    };
}
