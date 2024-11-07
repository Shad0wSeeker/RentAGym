using Ardalis.Specification;


namespace RentAGym.Application.Specifications.ScheduleSpecification
{
    public class RentByTenantSpecification:Specification<ReservedSchedule>
    {
        public RentByTenantSpecification(string tenantId)
        {
            Query.Where(r => r.TenantId == tenantId);
        }
    }
}
