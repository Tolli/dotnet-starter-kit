using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text;
using ClosedXML.Excel;
using CsvHelper;
using System.Globalization;

namespace FSH.Starter.Blazor.Client.Pages.Payments;

public partial class Payments
{
    [Inject]
    protected IApiClient Client { get; set; } = default!;

    // DialogService is injected globally from _Imports.razor

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    
    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    protected EntityServerTableContext<PaymentResponse, Guid, PaymentViewModel> Context { get; set; } = default!;

    private EntityTable<PaymentResponse, Guid, PaymentViewModel> _table = default!;

    private DateRange? dateRange;

    private CustomerResponse? _selectedCustomer;

    protected override void OnInitialized()
    {
        Context = new(
            entityName: "Payment",
            entityNamePlural: "Payments",
            entityResource: FshResources.Payments,
            fields: new()
            {
                new(payment => payment.Id, "Id", "Id"),
                new(payment => payment.CustomerId, "Customer SSN", "CustomerId", Template: CustomerSsnTemplate),
                new(payment => payment.Amount, "Amount", "Amount", Template: AmountTemplate),
                new(payment => payment.Currency, "Currency", "Currency"),
                new(payment => payment.Status, "Status", "Status", Template: StatusTemplate),
                new(payment => payment.Method, "Method", "Method"),
                new(payment => payment.PaymentDate, "Date", "PaymentDate", Type: typeof(DateTime), Template: DateTemplate),
                new(payment => payment.TransactionId, "Transaction", "TransactionId"),
                new(payment => payment.Description, "Description", "Description"),
            },
            enableAdvancedSearch: true,
            idFunc: payment => payment.Id,
            searchFunc: async filter =>
            {
                var paymentFilter = filter.Adapt<SearchPaymentsCommand>();
                
                // Apply date range filter
                if (dateRange != null)
                {
                    paymentFilter.FromDate = dateRange.Start;
                    paymentFilter.ToDate = dateRange.End;
                }
                
                var result = await Client.SearchPaymentsEndpointAsync("1", paymentFilter);
                return result.Adapt<PaginationResponse<PaymentResponse>>();
            },
            createFunc: async payment =>
            {
                if (_selectedCustomer?.Id == null)
                {
                    throw new InvalidOperationException("Please select a customer");
                }
                
                var command = payment.Adapt<CreatePaymentCommand>();
                command.CustomerId = _selectedCustomer.Id.Value;
                
                await Client.CreatePaymentEndpointAsync("1", command);
                Snackbar.Add("Payment created successfully", Severity.Success);
                
                _selectedCustomer = null; // Reset for next creation
            },
            updateFunc: async (id, payment) =>
            {
                await Client.UpdatePaymentEndpointAsync("1", id, payment.Adapt<UpdatePaymentCommand>());
                Snackbar.Add("Payment updated successfully", Severity.Success);
            },
            deleteFunc: async id =>
            {
                await Client.DeletePaymentEndpointAsync("1", id);
                Snackbar.Add("Payment deleted successfully", Severity.Success);
            });
    }

    private RenderFragment<PaymentResponse> AmountTemplate => payment => builder =>
    {
        builder.OpenComponent<MudText>(0);
        builder.AddAttribute(1, "Typo", Typo.body2);
        builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
        {
            childBuilder.OpenElement(3, "strong");
            childBuilder.AddContent(4, $"{payment.Amount:N2} {payment.Currency}");
            childBuilder.CloseElement();
        }));
        builder.CloseComponent();
    };

    private RenderFragment<PaymentResponse> CustomerSsnTemplate => payment => builder =>
    {
        builder.OpenComponent<MudText>(0);
        builder.AddAttribute(1, "Typo", Typo.body2);
        builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
        {
            childBuilder.AddContent(3, payment.CustomerId.ToString());
        }));
        builder.CloseComponent();
    };

    private async Task<IEnumerable<CustomerResponse>> SearchCustomers(string searchText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return Enumerable.Empty<CustomerResponse>();

        try
        {
            var filter = new SearchCustomersCommand
            {
                PageNumber = 1,
                PageSize = 10,
                SearchText = searchText
            };

            var result = await Client.SearchCustomersEndpointAsync("1", filter);
            return result?.Items ?? Enumerable.Empty<CustomerResponse>();
        }
        catch
        {
            return Enumerable.Empty<CustomerResponse>();
        }
    }

    private RenderFragment<PaymentResponse> StatusTemplate => payment => builder =>
    {
        var color = payment.Status switch
        {
            PaymentStatus._2 => Color.Success,      // Completed
            PaymentStatus._0 => Color.Warning,      // Pending
            PaymentStatus._1 => Color.Info,         // Processing
            PaymentStatus._3 => Color.Error,        // Failed
            PaymentStatus._4 => Color.Default,      // Cancelled
            PaymentStatus._5 => Color.Secondary,    // Refunded
            _ => Color.Default
        };

        builder.OpenComponent<MudChip<string>>(0);
        builder.AddAttribute(1, "Color", color);
        builder.AddAttribute(2, "Size", Size.Small);
        builder.AddAttribute(3, "ChildContent", (RenderFragment)(childBuilder =>
        {
            childBuilder.AddContent(4, GetStatusDisplayName(payment.Status));
        }));
        builder.CloseComponent();
    };

    private RenderFragment<PaymentResponse> DateTemplate => payment => builder =>
    {
        builder.OpenComponent<MudText>(0);
        builder.AddAttribute(1, "Typo", Typo.body2);
        builder.AddAttribute(2, "ChildContent", (RenderFragment)(childBuilder =>
        {
            childBuilder.AddContent(3, payment.PaymentDate.ToString("dd/MM/yyyy HH:mm"));
        }));
        builder.CloseComponent();
    };

    private async Task OpenImportDialog()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<ImportPaymentsDialog>("Import Payments", options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await _table.ReloadDataAsync();
        }
    }

    private async Task ExportPayments()
    {
        try
        {
            // Get all payments for export (you might want to add pagination or filtering)
            var filter = new SearchPaymentsCommand
            {
                PageNumber = 1,
                PageSize = 10000 // Large number to get all records
            };

            var payments = await Client.SearchPaymentsEndpointAsync("1", filter);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Payments");

            // Headers
            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Customer SSN";
            worksheet.Cell(1, 3).Value = "Invoice ID";
            worksheet.Cell(1, 4).Value = "Amount";
            worksheet.Cell(1, 5).Value = "Currency";
            worksheet.Cell(1, 6).Value = "Status";
            worksheet.Cell(1, 7).Value = "Method";
            worksheet.Cell(1, 8).Value = "Transaction ID";
            worksheet.Cell(1, 9).Value = "Description";
            worksheet.Cell(1, 10).Value = "Payment Date";
            worksheet.Cell(1, 11).Value = "Failure Reason";

            // Style headers
            var headerRange = worksheet.Range(1, 1, 1, 11);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data
            int row = 2;
            foreach (var payment in payments.Items)
            {
                // Fetch customer to get SSN
                string customerSSN = "N/A";
                try
                {
                    var customer = await Client.GetCustomerEndpointAsync("1", payment.CustomerId);
                    customerSSN = customer?.Ssn ?? "N/A";
                }
                catch
                {
                    // Customer not found or error
                }
                
                worksheet.Cell(row, 1).Value = payment.Id.ToString();
                worksheet.Cell(row, 2).Value = customerSSN;
                worksheet.Cell(row, 3).Value = payment.InvoiceId?.ToString() ?? "";
                worksheet.Cell(row, 4).Value = payment.Amount;
                worksheet.Cell(row, 5).Value = payment.Currency;
                worksheet.Cell(row, 6).Value = GetStatusDisplayName(payment.Status);
                worksheet.Cell(row, 7).Value = GetMethodDisplayName(payment.Method);
                worksheet.Cell(row, 8).Value = payment.TransactionId ?? "";
                worksheet.Cell(row, 9).Value = payment.Description ?? "";
                worksheet.Cell(row, 10).Value = payment.PaymentDate.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell(row, 11).Value = payment.FailureReason ?? "";
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            try
            {
                workbook.SaveAs(stream);
                stream.Position = 0; // Reset stream position to beginning

                // Download file using proper stream reference for Blazor WASM
                var fileName = $"Payments_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var streamRef = new DotNetStreamReference(stream: stream);
                
                await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
                
                // Give JavaScript time to read the stream before disposing
                await Task.Delay(3000);
                
                streamRef.Dispose();
                stream.Dispose();
            }
            catch
            {
                stream?.Dispose();
                throw;
            }

            Snackbar.Add($"Exported {payments.Items.Count} payments", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Export failed: {ex.Message}", Severity.Error);
        }
    }

    private static string GetStatusDisplayName(PaymentStatus status) => status switch
    {
        PaymentStatus._0 => "Pending",
        PaymentStatus._1 => "Processing",
        PaymentStatus._2 => "Completed",
        PaymentStatus._3 => "Failed",
        PaymentStatus._4 => "Cancelled",
        PaymentStatus._5 => "Refunded",
        _ => status.ToString()
    };

    private static string GetMethodDisplayName(PaymentMethod method) => method switch
    {
        PaymentMethod._0 => "Credit Card",
        PaymentMethod._1 => "Debit Card",
        PaymentMethod._2 => "Bank Transfer",
        PaymentMethod._3 => "Cash",
        PaymentMethod._4 => "PayPal",
        PaymentMethod._5 => "Stripe",
        PaymentMethod._99 => "Other",
        _ => method.ToString()
    };
}

public class PaymentViewModel : UpdatePaymentCommand
{
    public Guid CustomerId { get; set; }
    public Guid? InvoiceId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
}
