using FSH.Framework.Core.Caching;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
public sealed class GetGroupHandler(
    [FromKeyedServices("booking:groups")] IReadRepository<Group> repository,
    ICacheService cache)
    : IRequestHandler<GetGroupRequest, GroupResponse>
{
    public async Task<GroupResponse> Handle(GetGroupRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"group:{request.Id}",
            async () =>
            {
                var spec = new GetGroupSpecs(request.Id);
                var groupItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (groupItem == null) throw new GroupNotFoundException(request.Id);
                return groupItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
