using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Search.v1;
public class SearchGroupSpecs : EntitiesByPaginationFilterSpec<Group, GroupResponse>
{
    public SearchGroupSpecs(SearchGroupsCommand command)
        : base(command) =>
        Query
            .Include(p => p.Members)
            .Include(p => p.CourtRentals)
            .OrderBy(c => c.Name, !command.HasOrderBy())
            .Where(p => p.Members.Any(m => m.Customer.Id == command.ContactId!.Value), command.ContactId.HasValue)
            .Where(b => b.Name.Contains(command.Keyword), !string.IsNullOrEmpty(command.Keyword));
}
