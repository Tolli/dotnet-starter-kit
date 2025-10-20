using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
public sealed class GetGroupMemberHandler(
    [FromKeyedServices("booking:groupmembers")] IReadRepository<GroupMember> repository,
    ICacheService cache)
    : IRequestHandler<GetGroupMemberRequest, GroupMemberResponse>
{
    public async Task<GroupMemberResponse> Handle(GetGroupMemberRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"groupmember:{request.Id}",
            async () =>
            {
                var spec = new GetGroupMemberSpecs(request.Id);
                var groupmemberItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (groupmemberItem == null) throw new GroupMemberNotFoundException(request.Id);
                return groupmemberItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
