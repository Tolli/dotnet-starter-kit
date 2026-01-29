# Customer SSN Implementation for Payment Import and Display

## Summary
Modified the payment import and display functionality to use Customer SSN instead of Customer ID (GUID) for better usability.

## Changes Made

### 1. Import Dialog (`apps/blazor/client/Pages/Payments/ImportPaymentsDialog.razor`)

#### Updated Import Instructions
- Changed header text from "CustomerId" to "CustomerSSN"
- Users now provide SSN instead of GUID in Excel/CSV files

#### Modified PaymentImportModel
```csharp
// Before
public Guid CustomerId { get; set; }

// After
public string CustomerSSN { get; set; } = string.Empty;
```

#### Updated Excel Import Logic
- Added customer lookup by SSN before creating payment
- Searches for customer using `SearchCustomersCommand` with SSN as search text
- Validates customer exists before proceeding
- Provides clear error message if customer not found

#### Updated CSV Import Logic
- Similar changes as Excel import
- Looks up customer by SSN from import record
- Maps found customer ID to payment command

### 2. Payments Display (`apps/blazor/client/Pages/Payments/Payments.razor` & `.razor.cs`)

#### Updated Table Column
- Changed column header from "Customer" to "Customer SSN"
- Added custom `CustomerSsnTemplate` render fragment
- Template fetches customer data and displays SSN
- Displays "N/A" if customer not found or error occurs

#### Added Customer Autocomplete for Create/Edit
- Replaced Customer ID text field with MudAutocomplete component
- Users can search customers by SSN or name
- Displays customer SSN and name in dropdown
- Automatically populates CustomerId when customer selected
- Shows helper text: "Enter customer SSN or name to search"

#### Added SearchCustomers Method
```csharp
private async Task<IEnumerable<CustomerResponse>> SearchCustomers(string searchText, CancellationToken cancellationToken)
```
- Searches customers using partial text match
- Returns up to 10 matching customers
- Handles errors gracefully

#### Updated Create Function
- Validates that a customer is selected
- Extracts customer ID from selected customer
- Resets selection after successful creation

### 3. Export Functionality

#### Updated Export Headers
- Changed "Customer ID" to "Customer SSN" in Excel export

#### Updated Export Data
- Fetches customer data for each payment
- Exports customer SSN instead of raw GUID
- Handles errors gracefully with "N/A" fallback

## Benefits

1. **Better Usability**: Users work with familiar SSN values instead of GUIDs
2. **Easier Import**: No need to lookup customer GUIDs before import
3. **Clearer Display**: SSN is more meaningful than GUID in the UI
4. **Search Support**: Autocomplete makes it easy to find customers
5. **Error Handling**: Clear error messages when customer not found
6. **Maintains Backend Compatibility**: Still uses GUID internally for API calls

## Testing Recommendations

1. **Import Testing**
   - Test Excel import with valid SSNs
   - Test CSV import with valid SSNs
   - Test with invalid/non-existent SSNs
   - Verify error messages are clear

2. **Display Testing**
   - Verify SSN displays correctly in table
   - Test pagination with multiple payments
   - Verify "N/A" displays for missing customers

3. **Create/Edit Testing**
   - Test autocomplete search functionality
   - Verify customer selection populates correctly
   - Test creating payment with selected customer
   - Verify validation messages

4. **Export Testing**
   - Export payments and verify SSN column
   - Test with mix of valid and invalid customer IDs
   - Verify "N/A" for missing customers

## Sample Import File Format

### Excel/CSV Headers
```
CustomerSSN | InvoiceId | Amount | Currency | Method | Description
```

### Sample Data
```
1234567890 | | 100.00 | ISK | 0 | Test payment
9876543210 | guid-here | 250.50 | ISK | 1 | Another payment
```

## Known Considerations

1. **Performance**: Each row in table makes API call to fetch customer SSN
   - Consider caching customer data if performance becomes an issue
   - Could batch load customers for visible rows

2. **Customer Uniqueness**: Assumes SSN is unique per customer
   - Search uses first matching customer if multiple found

3. **API Dependency**: Requires customer search endpoint to be available
   - Graceful fallback to "N/A" if endpoint fails
