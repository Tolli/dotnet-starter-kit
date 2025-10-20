using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Delete.v1;
public sealed class DeleteGroupMemberHandler(
    ILogger<DeleteGroupMemberHandler> logger,
    [FromKeyedServices("booking:groupmembers")] IRepository<GroupMember> repository)
    : IRequestHandler<DeleteGroupMemberCommand>
{
    public async Task Handle(DeleteGroupMemberCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var groupmember = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = groupmember ?? throw new GroupMemberNotFoundException(request.Id);
        await repository.DeleteAsync(groupmember, cancellationToken);
        logger.LogInformation("groupmember with id : {GroupMemberId} deleted", groupmember.Id);
    }
}
