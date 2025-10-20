using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Search.v1;
public sealed class SearchGroupsHandler(
    [FromKeyedServices("booking:groups")] IReadRepository<Group> repository)
    : IRequestHandler<SearchGroupsCommand, PagedList<GroupResponse>>
{
    public async Task<PagedList<GroupResponse>> Handle(SearchGroupsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchGroupSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<GroupResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
