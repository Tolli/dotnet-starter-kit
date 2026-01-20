using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.Receipts.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Search.v1;
public class SearchReceiptSpecs : EntitiesByPaginationFilterSpec<Receipt, ReceiptResponse>
{
    public SearchReceiptSpecs(SearchReceiptsCommand command)
        : base(command) =>
        Query
            .OrderBy(c => c.GroupMember.Customer.Name, !command.HasOrderBy())
            .Where(p => p.AmountTotal >= command.MinimumRate!.Value, command.MinimumRate.HasValue)
            .Where(p => p.AmountTotal <= command.MaximumRate!.Value, command.MaximumRate.HasValue);
}
