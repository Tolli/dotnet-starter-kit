using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Create.v1;
public class CreateCourtRentalSessionCommandValidator : AbstractValidator<CreateCourtRentalSessionCommand>
{
    public CreateCourtRentalSessionCommandValidator()
    {
    }
}
