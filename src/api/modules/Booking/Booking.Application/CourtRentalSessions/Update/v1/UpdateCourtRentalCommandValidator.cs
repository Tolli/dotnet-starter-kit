using FluentValidation;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Update.v1;
public class UpdateCourtRentalSessionCommandValidator : AbstractValidator<UpdateCourtRentalSessionCommand>
{
    public UpdateCourtRentalSessionCommandValidator()
    {
    }
}
