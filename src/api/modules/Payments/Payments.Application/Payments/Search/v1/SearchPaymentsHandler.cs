using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;
using FSH.Starter.WebApi.Payments.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Search.v1;

public sealed class SearchPaymentsHandler(
    [FromKeyedServices("payments:payments")] IReadRepository<Payment> repository)
    : IRequestHandler<SearchPaymentsCommand, PagedList<PaymentResponse>>
{
    public async Task<PagedList<PaymentResponse>> Handle(SearchPaymentsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var spec = new SearchPaymentSpecs(request);
        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<PaymentResponse>(items, request.PageNumber, request.PageSize, totalCount);
    }
}
