using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Search.v1;
public sealed class SearchGroupMembersHandler(
    [FromKeyedServices("booking:groupmembers")] IReadRepository<GroupMember> repository)
    : IRequestHandler<SearchGroupMembersCommand, PagedList<GroupMemberResponse>>
{
    public async Task<PagedList<GroupMemberResponse>> Handle(SearchGroupMembersCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchGroupMemberSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<GroupMemberResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

