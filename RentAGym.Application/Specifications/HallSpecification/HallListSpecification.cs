using Ardalis.Specification;
using RentAGym.Application.Filters;
using RentAGym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentAGym.Application.Specifications.HallSpecification
{
    public class HallListSpecification : Specification<Hall>
    {
        public HallListSpecification(HallListFilter filter)
        {

            Query
             .Include(h => h.Options)
             .Include(h => h.Images)
             .Where(h => filter.TypeId == 0 || h.HallTypeId == filter.TypeId)
             .Where(h => filter.RegionId == 0 || h.Facility.RegionId == filter.RegionId)
             .Where(h =>
                 h.Payment == filter.PaymentType &&
                 h.Area >= filter.SquareFrom &&
                 h.Area <= (filter.SquareTo ?? double.MaxValue) &&
                 h.BasePrice >= filter.PriceFrom &&
                 h.BasePrice <= (filter.PriceTo ?? double.MaxValue));

            // Фильтрация по OptionIds, если заданы
            if (filter.OptionIds != null && filter.OptionIds.Any())
            {
                var requiredOptionIds = filter.OptionIds;
                var requiredCount = requiredOptionIds.Count;

                Query.Where(h =>
                    h.Options.Select(o => o.Id).Intersect(requiredOptionIds).Count() == requiredCount);
            }

            // Фильтрация по Timestamp
            if (filter.Timestamp != null)
            {
                var ts = filter.Timestamp.Value;
                Query.Where(h =>
                    !h.ReservedSchedules.Any(sc =>
                        sc.ReservedHour.Date == ts.Date &&
                        sc.ReservedHour.Hour == ts.Hour));
            }

            // Фильтрация по TimeFrom / TimeTo
            if (filter.TimeFrom != null || filter.TimeTo != null)
            {
                if (filter.TimeFrom != null && filter.TimeTo == null)
                {
                    Query.Where(h => !h.ReservedSchedules.Any(sc => sc.ReservedHour >= filter.TimeFrom));
                }
                else if (filter.TimeFrom == null && filter.TimeTo != null)
                {
                    Query.Where(h => !h.ReservedSchedules.Any(sc => sc.ReservedHour <= filter.TimeTo));
                }
                else if (filter.TimeFrom != null && filter.TimeTo != null)
                {
                    Query.Where(h =>
                        !h.ReservedSchedules.Any(sc =>
                            sc.ReservedHour >= filter.TimeFrom &&
                            sc.ReservedHour <= filter.TimeTo));
                }
            }
        }
    }
}
