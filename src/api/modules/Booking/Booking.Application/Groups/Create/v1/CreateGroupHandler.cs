using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Create.v1;
public sealed class CreateGroupHandler(
    ILogger<CreateGroupHandler> logger,
    [FromKeyedServices("booking:groups")] IRepository<Group> repository)
    : IRequestHandler<CreateGroupCommand, CreateGroupResponse>
{
    public async Task<CreateGroupResponse> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var group = Group.Create(request.Name!, request.StartDate, request.EndDate);
        await repository.AddAsync(group, cancellationToken);
        logger.LogInformation("group created {GroupId}", group.Id);
        return new CreateGroupResponse(group.Id);
    }
}
