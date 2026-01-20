using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Create.v1;
public sealed class CreateGroupMemberHandler(
    ILogger<CreateGroupMemberHandler> logger,
    [FromKeyedServices("booking:groupmembers")] IRepository<GroupMember> repository)
    : IRequestHandler<CreateGroupMemberCommand, CreateGroupMemberResponse>
{
    public async Task<CreateGroupMemberResponse> Handle(CreateGroupMemberCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var customer = GroupMember.Create(request.GroupId, request.CustomerId, request.Amount, request.Discount, request.IsContact);
        await repository.AddAsync(customer, cancellationToken);
        logger.LogInformation("group member created {GroupMemberId}", customer.Id);
        return new CreateGroupMemberResponse(customer.Id);
    }
}
