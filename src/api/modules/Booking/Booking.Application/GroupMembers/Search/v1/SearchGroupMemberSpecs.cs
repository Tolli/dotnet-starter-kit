using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Search.v1;
public class SearchGroupMemberSpecs : EntitiesByPaginationFilterSpec<GroupMember, GroupMemberResponse>
{
    public SearchGroupMemberSpecs(SearchGroupMembersCommand command)
        : base(command) =>
        Query
            .OrderBy(c => c.Customer.Name, !command.HasOrderBy())
            .Where(p => p.Price >= command.MinimumRate!.Value, command.MinimumRate.HasValue)
            .Where(p => p.Price <= command.MaximumRate!.Value, command.MaximumRate.HasValue);
}
