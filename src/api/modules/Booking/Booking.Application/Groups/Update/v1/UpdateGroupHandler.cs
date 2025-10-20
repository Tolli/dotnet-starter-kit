using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Update.v1;
public sealed class UpdateGroupHandler(
    ILogger<UpdateGroupHandler> logger,
    [FromKeyedServices("booking:groups")] IRepository<Group> repository)
    : IRequestHandler<UpdateGroupCommand, UpdateGroupResponse>
{
    public async Task<UpdateGroupResponse> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var group = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = group ?? throw new GroupNotFoundException(request.Id);
        var updatedGroup = group.Update(request.Name, request.StartDate, request.EndDate, request.ContactId);
        await repository.UpdateAsync(updatedGroup, cancellationToken);
        logger.LogInformation("Group with id : {GroupId} updated.", group.Id);
        return new UpdateGroupResponse(group.Id);
    }
}
