using MediatR;
using RentAGym.Application.Dto;
using RentAGym.Application.Interfaces;
using RentAGym.Application.Specifications.HallSpecification;
using RentAGym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RentAGym.Application.CommonUseCases
{
    public sealed class GetHallListHandler : IRequestHandler<GetHallListRequest, IEnumerable<HallListRequestDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetHallListHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<HallListRequestDTO>> Handle(GetHallListRequest request, CancellationToken cancellationToken)
        {

            var repository = _unitOfWork.HallRepository;

            var response = await repository.ListAsync(
                new HallListSpecification(request.filter), cancellationToken);

            // Фильтрация по OptionIds, если заданы
            if (request.filter.OptionIds != null && request.filter.OptionIds.Any())
            {
                var requiredOptionIds = request.filter.OptionIds;
                var requiredCount = requiredOptionIds.Count;

                response = response.Where(h =>
                    h.Options.Select(o => o.Id).Intersect(requiredOptionIds).Count() == requiredCount).ToList();
            }

            return _mapper.Map<IEnumerable<HallListRequestDTO>>(response);
        }
    }
}
