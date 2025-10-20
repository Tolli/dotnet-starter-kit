using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Delete.v1;
public sealed class DeleteGroupHandler(
    ILogger<DeleteGroupHandler> logger,
    [FromKeyedServices("booking:groups")] IRepository<Group> repository)
    : IRequestHandler<DeleteGroupCommand>
{
    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var group = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = group ?? throw new GroupNotFoundException(request.Id);
        await repository.DeleteAsync(group, cancellationToken);
        logger.LogInformation("Group with id : {GroupId} deleted", group.Id);
    }
}
