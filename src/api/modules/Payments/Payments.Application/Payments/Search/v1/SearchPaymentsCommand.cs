using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;
using FSH.Starter.WebApi.Payments.Domain;
using MediatR;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Search.v1;

public class SearchPaymentsCommand : PaginationFilter, IRequest<PagedList<PaymentResponse>>
{
    public Guid? CustomerId { get; set; }
    public Guid? InvoiceId { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentMethod? Method { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
