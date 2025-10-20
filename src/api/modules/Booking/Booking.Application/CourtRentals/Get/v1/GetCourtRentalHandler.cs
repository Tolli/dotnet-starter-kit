using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
public sealed class GetCourtRentalHandler(
    [FromKeyedServices("booking:courtrentals")] IReadRepository<CourtRental> repository,
    ICacheService cache)
    : IRequestHandler<GetCourtRentalRequest, CourtRentalResponse>
{
    public async Task<CourtRentalResponse> Handle(GetCourtRentalRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"courtrental:{request.Id}",
            async () =>
            {
                var spec = new GetCourtRentalSpecs(request.Id);
                var courtrentalItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (courtrentalItem == null) throw new CourtRentalNotFoundException(request.Id);
                return courtrentalItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
