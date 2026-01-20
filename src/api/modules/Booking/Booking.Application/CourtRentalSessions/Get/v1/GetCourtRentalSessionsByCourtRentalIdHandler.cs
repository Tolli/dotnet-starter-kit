using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
public sealed class GetCourtRentalSessionsByCourtRentalIdHandler(
    [FromKeyedServices("booking:courtrentalsessions")] IReadRepository<CourtRentalSession> repository)
    : IRequestHandler<GetCourtRentalSessionsByCourtRentalIdCommand, PagedList<CourtRentalSessionResponse>>
{
    public async Task<PagedList<CourtRentalSessionResponse>> Handle(GetCourtRentalSessionsByCourtRentalIdCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new GetCourtRentalSessionsByCourtRentalIdSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<CourtRentalSessionResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
