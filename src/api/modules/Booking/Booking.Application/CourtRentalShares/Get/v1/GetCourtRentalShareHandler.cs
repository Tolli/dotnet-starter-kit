using Ardalis.Specification;
using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Get.v1;
public class GetCourtRentalShareRequest : IRequest<CourtRentalShareResponse>
{
    public Guid Id { get; set; }
    public GetCourtRentalShareRequest(Guid id) => Id = id;
}

public class GetCourtRentalSharesByCourtRentalIdCommand : PaginationFilter, IRequest<PagedList<CourtRentalShareResponse>>
{
    public Guid CourtRentalId { get; set; }
}
public class GetCourtRentalShareSpecs : Specification<CourtRentalShare, CourtRentalShareResponse>
{
    public GetCourtRentalShareSpecs(Guid id)
    {
        Query
            .Include(p => p.Customer)
            .Where(p => p.Id == id);
    }
}

public class GetCourtRentalSharesByCourtRentalIdSpecs : EntitiesByPaginationFilterSpec<CourtRentalShare, CourtRentalShareResponse>
{
    public GetCourtRentalSharesByCourtRentalIdSpecs(GetCourtRentalSharesByCourtRentalIdCommand command)
    : base(command) =>
        Query
            .Include(p => p.Customer)
            .Where(p => p.CourtRentalId == command.CourtRentalId);
}

public sealed class GetCourtRentalShareHandler(
    [FromKeyedServices("booking:courtrentalshares")] IReadRepository<CourtRentalShare> repository,
    ICacheService cache)
    : IRequestHandler<GetCourtRentalShareRequest, CourtRentalShareResponse>
{
    public async Task<CourtRentalShareResponse> Handle(GetCourtRentalShareRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"courtrentalshare:{request.Id}",
            async () =>
            {
                var spec = new GetCourtRentalShareSpecs(request.Id);
                var courtrentalshareItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (courtrentalshareItem == null) throw new CourtRentalShareNotFoundException(request.Id);
                return courtrentalshareItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

public sealed class GetCourtRentalSharesByCourtRentalIdHandler(
    [FromKeyedServices("booking:courtrentalshares")] IReadRepository<CourtRentalShare> repository,
    ICacheService cache)
    : IRequestHandler<GetCourtRentalSharesByCourtRentalIdCommand, PagedList<CourtRentalShareResponse>>
{
    public async Task<PagedList<CourtRentalShareResponse>> Handle(GetCourtRentalSharesByCourtRentalIdCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new GetCourtRentalSharesByCourtRentalIdSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<CourtRentalShareResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
