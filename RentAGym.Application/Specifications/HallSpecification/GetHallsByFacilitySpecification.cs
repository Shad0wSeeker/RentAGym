using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentAGym.Application.Specifications.HallSpecification
{
    public class GetHallsByFacilitySpecification : Specification<Hall>
    {
        public GetHallsByFacilitySpecification(int facilityId)
        {
            Query.Where(h => h.FacilityId == facilityId)
                .Include(h => h.Images);
        }
    }
}
