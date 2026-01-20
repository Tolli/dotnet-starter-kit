using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.Receipts.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Booking.Application.Receipts.Search.v1;
public sealed class SearchReceiptsHandler(
    [FromKeyedServices("booking:receipts")] IReadRepository<Receipt> repository)
    : IRequestHandler<SearchReceiptsCommand, PagedList<ReceiptResponse>>
{
    public async Task<PagedList<ReceiptResponse>> Handle(SearchReceiptsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchReceiptSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<ReceiptResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

