using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Create.v1;
public sealed record CreateGroupCommand(
    [property: DefaultValue("Sample Group")] string? Name,
    [property: DefaultValue("2025-09-01")] DateTime StartDate = default,
    [property: DefaultValue("2026-05-31")] DateTime EndDate = default,
    [property: DefaultValue(null)] Guid? ContactId = null) : IRequest<CreateGroupResponse>;

