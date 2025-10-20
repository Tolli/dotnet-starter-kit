using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Update.v1;
public sealed class UpdateGroupMemberHandler(
    ILogger<UpdateGroupMemberHandler> logger,
    [FromKeyedServices("booking:groupmembers")] IRepository<GroupMember> repository)
    : IRequestHandler<UpdateGroupMemberCommand, UpdateGroupMemberResponse>
{
    public async Task<UpdateGroupMemberResponse> Handle(UpdateGroupMemberCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var groupmember = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = groupmember ?? throw new GroupMemberNotFoundException(request.Id);
        var updatedGroupMember = groupmember.Update(request.GroupId, request.CustomerId, request.Price, request.Percentage, request.IsContact);
        await repository.UpdateAsync(updatedGroupMember, cancellationToken);
        logger.LogInformation("groupmember with id : {GroupMemberId} updated.", groupmember.Id);
        return new UpdateGroupMemberResponse(groupmember.Id);
    }
}
