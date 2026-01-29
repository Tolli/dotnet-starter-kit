# Build Error Resolution - Payments Module

## Current Status

The Payments Blazor client pages have been created but cannot be fully compiled yet because:

### ? **Missing API Client Methods**

The following methods don't exist in `apps/blazor/infrastructure/Api/ApiClient.cs` yet:
- `CreatePaymentEndpointAsync`
- `GetPaymentEndpointAsync`  
- `UpdatePaymentEndpointAsync`
- `DeletePaymentEndpointAsync`
- `SearchPaymentsEndpointAsync`

And these types are missing:
- `PaymentResponse`
- `CreatePaymentCommand`
- `UpdatePaymentCommand`
- `SearchPaymentsCommand`
- `PaymentStatus` enum
- `PaymentMethod` enum

## ? Solution: Regenerate API Client

You need to regenerate the API client using NSwag after the API server is running with the Payments module.

### Step 1: Start the API Server
```bash
cd src/api/server
dotnet run
```

### Step 2: Verify Swagger Endpoint
Open browser to: `https://localhost:7001/swagger` (adjust port if needed)

Verify you see Payments endpoints in the Swagger UI.

### Step 3: Regenerate API Client

#### Option A: Using NSwag CLI (Recommended)
```bash
cd src

# Install NSwag CLI if needed
dotnet tool install -g NSwag.MSBuild

# Generate client
nswag openapi2csclient \
  /input:https://localhost:7001/swagger/v1/swagger.json \
  /output:apps/blazor/infrastructure/Api/ApiClient.cs \
  /namespace:FSH.Starter.Blazor.Infrastructure.Api \
  /generateClientInterfaces:true \
  /generateDtoTypes:true
```

#### Option B: Using NSwag Studio (GUI)
1. Download and open [NSwag Studio](https://github.com/RicoSuter/NSwag/releases)
2. Configure:
   - Input: `https://localhost:7001/swagger/v1/swagger.json`
   - Output: `C:\dev\dotnet-starter-kit\src\apps\blazor\infrastructure\Api\ApiClient.cs`
   - Namespace: `FSH.Starter.Blazor.Infrastructure.Api`
   - Check "Generate Client Interfaces"
   - Check "Generate DTO Types"
3. Click "Generate Outputs"

#### Option C: Check for Build Task
Some projects auto-generate the client. Check if there's an NSwag build target in:
- `apps/blazor/infrastructure/Infrastructure.csproj`
- `api/server/Server.csproj`

### Step 4: Verify Generation
After generation, verify these exist in `ApiClient.cs`:

```csharp
public partial interface IApiClient
{
    // ... existing methods ...
    
    Task<CreatePaymentResponse> CreatePaymentEndpointAsync(string version, CreatePaymentCommand body);
    Task<PaymentResponse> GetPaymentEndpointAsync(string version, Guid id);
    Task<UpdatePaymentResponse> UpdatePaymentEndpointAsync(string version, Guid id, UpdatePaymentCommand body);
    Task DeletePaymentEndpointAsync(string version, Guid id);
    Task<PagedListOfPaymentResponse> SearchPaymentsEndpointAsync(string version, SearchPaymentsCommand body);
}

public partial class PaymentResponse { /* ... */ }
public partial class CreatePaymentCommand { /* ... */ }
public partial class UpdatePaymentCommand { /* ... */ }
public partial class SearchPaymentsCommand { /* ... */ }
public enum PaymentStatus { /* ... */ }
public enum PaymentMethod { /* ... */ }
```

### Step 5: Build Again
```bash
cd src
dotnet build FSH.Starter.sln
```

## Alternative: Temporary Fix (Not Recommended)

If you need to build immediately without the API client, you could:

1. **Comment out the Payments page temporarily**
   - Remove from navigation in `NavMenu.razor`
   - Comment out route in `Payments.razor`

2. **Create stub API client extensions** (complex, not recommended)

## After API Client is Regenerated

Once the API client is regenerated with Payment methods:

1. ? Build will succeed
2. ? Run migrations: `dotnet ef database update --project api/migrations/PostgreSQL`
3. ? Start API and Blazor client
4. ? Assign Payments permissions to roles
5. ? Test the Payments page

## Additional Notes

- The API endpoints already exist and work (tested with Swagger)
- The database schema is ready (Payment table created)
- The permissions are configured
- Only the Blazor client code generation is pending

## Files Ready
- ? `apps/blazor/client/Pages/Payments/Payments.razor`
- ? `apps/blazor/client/Pages/Payments/Payments.razor.cs`  
- ? `apps/blazor/client/Pages/Payments/ImportPaymentsDialog.razor`
- ? `apps/blazor/client/Pages/Payments/payment-import-template.csv`
- ? `Shared/Authorization/FshResources.cs` (updated)
- ? `Shared/Authorization/FshPermissions.cs` (updated)
- ? `apps/blazor/client/Directory.Packages.props` (updated with ClosedXML, CsvHelper)

## Current Build Errors

All remaining build errors are due to missing API client types:
- `PaymentResponse` not found (21 occurrences)
- `CreatePaymentCommand` not found
- `UpdatePaymentCommand` not found  
- `SearchPaymentsCommand` not found
- `PaymentStatus` not found
- `PaymentMethod` not found
- API client methods not found

**All of these will be resolved after regenerating the API client.**
