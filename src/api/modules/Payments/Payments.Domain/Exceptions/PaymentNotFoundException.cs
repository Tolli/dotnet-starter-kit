using FSH.Framework.Core.Exceptions;
using System.Net;

namespace FSH.Starter.WebApi.Payments.Domain.Exceptions;

public sealed class PaymentNotFoundException : CustomException
{
    public PaymentNotFoundException(Guid id)
        : base($"Payment with id {id} not found.", null, HttpStatusCode.NotFound)
    {
    }
}
