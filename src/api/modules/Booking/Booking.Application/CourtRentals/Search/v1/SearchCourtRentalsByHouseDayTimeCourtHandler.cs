using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;
public sealed class SearchCourtRentalsByHouseDayTimeCourtHandler(
    [FromKeyedServices("booking:courtrentals")] IReadRepository<CourtRental> repository)
    : IRequestHandler<SearchCourtRentalsByHouseDayTimeCourtCommand, PagedList<CourtRentalResponse>>
{
    public async Task<PagedList<CourtRentalResponse>> Handle(SearchCourtRentalsByHouseDayTimeCourtCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchCourtRentalByHouseDayTimeCourtSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<CourtRentalResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

