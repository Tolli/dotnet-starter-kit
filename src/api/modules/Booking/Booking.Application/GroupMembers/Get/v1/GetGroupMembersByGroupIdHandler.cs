using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Search.v1;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
public sealed class GetGroupMembersByGroupIdHandler(
    [FromKeyedServices("booking:groupmembers")] IReadRepository<GroupMember> repository)
    : IRequestHandler<GetGroupMembersByGroupIdCommand, PagedList<GroupMemberResponse>>
{
    public async Task<PagedList<GroupMemberResponse>> Handle(GetGroupMembersByGroupIdCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new GetGroupMembersByGroupIdSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<GroupMemberResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}
