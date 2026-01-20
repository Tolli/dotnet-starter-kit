using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Search.v1;

public class SearchCourtRentalSharesCommand : PaginationFilter, IRequest<PagedList<CourtRentalShareResponse>>
{
    public Guid? GroupId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
}

public class SearchCourtRentalShareSpecs : EntitiesByPaginationFilterSpec<CourtRentalShare, CourtRentalShareResponse>
{
    public SearchCourtRentalShareSpecs(SearchCourtRentalSharesCommand command)
        : base(command) =>
        Query
            .Where(p => p.AmountTotal >= command.MinimumRate!.Value, command.MinimumRate.HasValue)
            .Where(p => p.AmountTotal <= command.MaximumRate!.Value, command.MaximumRate.HasValue);
}
public sealed class SearchCourtRentalSharesHandler(
    [FromKeyedServices("booking:courtrentalshares")] IReadRepository<CourtRentalShare> repository)
    : IRequestHandler<SearchCourtRentalSharesCommand, PagedList<CourtRentalShareResponse>>
{
    public async Task<PagedList<CourtRentalShareResponse>> Handle(SearchCourtRentalSharesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchCourtRentalShareSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<CourtRentalShareResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

