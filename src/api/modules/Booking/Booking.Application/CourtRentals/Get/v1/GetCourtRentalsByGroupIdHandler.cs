using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
public sealed class GetCourtRentalsByGroupIdHandler(
    [FromKeyedServices("booking:courtrentals")] IReadRepository<CourtRental> repository)
    : IRequestHandler<GetCourtRentalsByGroupIdCommand, PagedList<CourtRentalResponse>>
{
    public async Task<PagedList<CourtRentalResponse>> Handle(GetCourtRentalsByGroupIdCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new GetCourtRentalsByGroupIdSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<CourtRentalResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
