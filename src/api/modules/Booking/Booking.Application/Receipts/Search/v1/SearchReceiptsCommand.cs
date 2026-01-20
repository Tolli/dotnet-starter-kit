using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.Receipts.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Search.v1;

public class SearchReceiptsCommand : PaginationFilter, IRequest<PagedList<ReceiptResponse>>
{
    public Guid? GroupId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
}
