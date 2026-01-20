using Ardalis.Specification;
using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Get.v1;
public class GetReceiptRequest : IRequest<ReceiptResponse>
{
    public Guid Id { get; set; }
    public GetReceiptRequest(Guid id) => Id = id;
}
public class GetReceiptSpecs : Specification<Receipt, ReceiptResponse>
{
    public GetReceiptSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id);
    }
}

public sealed class GetReceiptHandler(
    [FromKeyedServices("booking:receipts")] IReadRepository<Receipt> repository,
    ICacheService cache)
    : IRequestHandler<GetReceiptRequest, ReceiptResponse>
{
    public async Task<ReceiptResponse> Handle(GetReceiptRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"receipt:{request.Id}",
            async () =>
            {
                var spec = new GetReceiptSpecs(request.Id);
                var receiptItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (receiptItem == null) throw new ReceiptNotFoundException(request.Id);
                return receiptItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
