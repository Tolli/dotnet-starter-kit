using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Search.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Search.v1;
public sealed class SearchCourtRentalSessionsHandler(
    [FromKeyedServices("booking:courtrentalsessions")] IReadRepository<CourtRentalSession> repository)
    : IRequestHandler<SearchCourtRentalSessionsCommand, PagedList<CourtRentalSessionResponse>>
{
    public async Task<PagedList<CourtRentalSessionResponse>> Handle(SearchCourtRentalSessionsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchCourtRentalSessionSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<CourtRentalSessionResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

