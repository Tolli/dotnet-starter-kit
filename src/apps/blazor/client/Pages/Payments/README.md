# Payments Module - Blazor Client Setup

## Overview
This guide explains how to complete the setup of the Payments module in the Blazor WebAssembly client.

## What Was Created

### 1. Blazor Pages
- **`apps/blazor/client/Pages/Payments/Payments.razor`** - Main payments management page
- **`apps/blazor/client/Pages/Payments/Payments.razor.cs`** - Code-behind with CRUD operations
- **`apps/blazor/client/Pages/Payments/ImportPaymentsDialog.razor`** - Dialog for importing payments from Excel/CSV

### 2. Features Implemented
? List all payments in descending chronological order
? Create new payments
? Edit existing payments
? Delete payments
? Advanced search with filters (Status, Method, Date Range)
? Import from Excel (.xlsx, .xls) or CSV files
? Export to Excel
? Status badges with color coding
? Permission-based access control

### 3. Dependencies Added
- **ClosedXML** - For Excel file generation and parsing
- **CsvHelper** - For CSV file parsing

## Setup Steps

### Step 1: Regenerate API Client

The Blazor client uses an auto-generated API client (NSwag). After adding the Payments module to the API, you need to regenerate it.

#### Option A: Using NSwag Studio (Recommended)
1. Make sure the API server is running (`dotnet run --project api/server/Server.csproj`)
2. Open NSwag Studio
3. Load the configuration file (if one exists) or create new:
   - **Input**: `https://localhost:7001/swagger/v1/swagger.json` (adjust port if needed)
   - **Output**: `apps/blazor/infrastructure/Api/ApiClient.cs`
   - Select **CSharp Client** template
   - Configure namespace: `FSH.Starter.Blazor.Infrastructure.Api`
4. Click **Generate Outputs**

#### Option B: Using NSwag CLI
```bash
# Install NSwag.MSBuild if not already installed
dotnet tool install -g NSwag.MSBuild

# Run from solution root
cd src
nswag openapi2csclient /input:https://localhost:7001/swagger/v1/swagger.json /output:apps/blazor/infrastructure/Api/ApiClient.cs /namespace:FSH.Starter.Blazor.Infrastructure.Api /generateClientInterfaces:true
```

#### Option C: Using the API Server
Some projects include a build task that auto-generates the client. Check `api/server/Server.csproj` for NSwag configuration.

### Step 2: Verify API Client Methods

After regenerating, verify these methods exist in `ApiClient.cs`:
- `CreatePaymentEndpointAsync`
- `GetPaymentEndpointAsync`
- `UpdatePaymentEndpointAsync`
- `DeletePaymentEndpointAsync`
- `SearchPaymentsEndpointAsync`

### Step 3: Add Navigation Permission Check

Update `apps/blazor/client/Layout/NavMenu.razor.cs` to add permission check:

```csharp
private bool _canViewPayments;

protected override async Task OnInitializedAsync()
{
    // ... existing code ...
    _canViewPayments = await AuthorizationService.HasPermissionAsync(FshResources.Payments, FshActions.View);
}
```

Then wrap the nav link in the razor file:
```razor
@if (_canViewPayments)
{
    <MudNavLink Href="/payments" Icon="@Icons.Material.Filled.Payment" Class="fsh-nav-child">Payments</MudNavLink>
}
```

### Step 4: Create Database Migration

```bash
# Navigate to solution root
cd src

# Create migration for Payments module
dotnet ef migrations add AddPaymentsModule --project api/migrations/PostgreSQL --context PaymentsDbContext --output-dir Migrations/Payments

# Or for SQL Server
dotnet ef migrations add AddPaymentsModule --project api/migrations/MSSQL --context PaymentsDbContext --output-dir Migrations/Payments
```

### Step 5: Run and Test

1. **Start the API**:
   ```bash
   cd src/api/server
   dotnet run
   ```

2. **Start the Blazor Client**:
   ```bash
   cd src/apps/blazor/client
   dotnet run
   ```

3. **Login and Navigate** to Payments (should be in the navigation menu)

4. **Assign Permissions**:
   - Go to Administration > Roles
   - Edit a role and assign Payments permissions:
     - View Payments
     - Create Payments
     - Update Payments
     - Delete Payments
     - Export Payments

## Usage Guide

### Creating a Payment
1. Click the **"+"** button in the Payments table
2. Fill in:
   - Customer ID (GUID)
   - Invoice ID (optional, GUID)
   - Amount (must be > 0)
   - Currency (3-letter code, e.g., ISK, USD)
   - Payment Method (dropdown)
   - Description (optional)
3. Click **Save**

### Importing from Excel/CSV

#### Excel Format (.xlsx)
Create a file with these columns:
| CustomerId | InvoiceId | Amount | Currency | Method | Description |
|------------|-----------|--------|----------|---------|-------------|
| guid-here  | guid-here | 100.00 | ISK      | BankTransfer | Payment for service |

#### CSV Format (.csv)
```csv
CustomerId,InvoiceId,Amount,Currency,Method,Description
11111111-1111-1111-1111-111111111111,,100.00,ISK,BankTransfer,Monthly payment
22222222-2222-2222-2222-222222222222,33333333-3333-3333-3333-333333333333,250.50,USD,CreditCard,One-time payment
```

**Import Steps:**
1. Click **"Import from Excel/CSV"** button
2. Select your Excel or CSV file
3. Click **Import**
4. Review results - successful imports and any errors will be displayed

### Exporting to Excel
1. Click **"Export to Excel"** button
2. File downloads automatically with name `Payments_YYYYMMDD_HHmmss.xlsx`
3. Includes all payment data with formatting

### Advanced Search
Click the **filter icon** to expand advanced search:
- **Status**: Filter by payment status (Pending, Completed, Failed, etc.)
- **Method**: Filter by payment method (CreditCard, BankTransfer, etc.)
- **Date Range**: Select start and end dates

## Payment Status Flow

```
Pending ? Processing ? Completed
                    ? Failed
                    ? Cancelled
                                 ? Refunded
```

- **Pending**: Initial state when payment is created
- **Processing**: Payment is being processed (future enhancement)
- **Completed**: Payment successfully processed
- **Failed**: Payment processing failed
- **Cancelled**: Payment was cancelled
- **Refunded**: Payment was refunded (from Completed state)

## Troubleshooting

### API Client Methods Not Found
**Solution**: Regenerate the API client (see Step 1)

### "Permission Denied" Error
**Solution**: Assign Payments permissions to your role (see Step 5.4)

### Import Errors
Common issues:
- **Invalid GUID format**: Ensure CustomerIds and InvoiceIds are valid GUIDs
- **Invalid amount**: Amount must be a positive decimal number
- **Invalid method**: Method must match PaymentMethod enum values exactly

### Database Migration Fails
**Solution**: Ensure connection string is correct and database server is running

## API Endpoints

The Payments module exposes these endpoints:

- **POST** `/api/v1/payments` - Create payment
- **GET** `/api/v1/payments/{id}` - Get payment by ID
- **PUT** `/api/v1/payments/{id}` - Update payment
- **DELETE** `/api/v1/payments/{id}` - Delete payment
- **POST** `/api/v1/payments/search` - Search payments (paginated)

## Next Steps

### Potential Enhancements
1. **Payment Gateway Integration**: Integrate with Stripe, PayPal, or local payment processors
2. **Webhooks**: Handle payment gateway webhooks for status updates
3. **Recurring Payments**: Add support for subscription-based recurring payments
4. **Payment Plans**: Create installment payment plans
5. **Reports**: Generate payment reports and analytics
6. **Email Notifications**: Send email confirmations when payments are processed
7. **Customer Integration**: Link directly with Customer records for easier selection
8. **Invoice Integration**: If you have an invoicing system, integrate payments with invoices

## Support

For issues or questions:
1. Check the application logs in `api/server/Logs`
2. Use browser developer tools (F12) to check for console errors
3. Verify API is running and accessible
4. Confirm database migrations have been applied

## File Locations

```
src/
??? api/
?   ??? modules/Payments/
?       ??? Payments.Domain/
?       ??? Payments.Application/
?       ??? Payments.Infrastructure/
??? apps/blazor/
?   ??? client/Pages/Payments/
?   ?   ??? Payments.razor
?   ?   ??? Payments.razor.cs
?   ?   ??? ImportPaymentsDialog.razor
?   ??? infrastructure/Api/
?       ??? ApiClient.cs (needs regeneration)
??? Shared/Authorization/
    ??? FshResources.cs (updated)
    ??? FshPermissions.cs (updated)
```
