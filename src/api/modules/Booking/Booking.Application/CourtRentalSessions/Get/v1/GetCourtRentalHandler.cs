using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
public sealed class GetCourtRentalSessionHandler(
    [FromKeyedServices("booking:courtrentalsessions")] IReadRepository<CourtRentalSession> repository,
    ICacheService cache)
    : IRequestHandler<GetCourtRentalSessionRequest, CourtRentalSessionResponse>
{
    public async Task<CourtRentalSessionResponse> Handle(GetCourtRentalSessionRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"courtrentalsession:{request.Id}",
            async () =>
            {
                var spec = new GetCourtRentalSessionSpecs(request.Id);
                var courtrentalSessionItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (courtrentalSessionItem == null) throw new CourtRentalSessionNotFoundException(request.Id);
                return courtrentalSessionItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
