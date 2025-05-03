using RentAGym.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentAGym.Application.LandLordUseCases
{
    public sealed class CreateHallHandler : IRequestHandler<CreateHallRequest,bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateHallHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(CreateHallRequest request, CancellationToken cancellationToken)
        {
            var tempHall = _mapper.Map<Hall>(request.requestDTO);
            try
            {
                var fac = await _unitOfWork.FacilityRepository.GetByIdAsync(request.requestDTO.FacilityId);
                var options = await _unitOfWork.OptionRepository.ListAsync();

                tempHall.LandlordId = fac.LandLordId;    //!!!
                tempHall.Facility = fac;
                tempHall.FacilityId = fac.Id;
                tempHall.Options = options.Where(o => tempHall.Options.Contains(o)).ToList();

                var hall = await _unitOfWork.HallRepository.AddAsync(tempHall);
                foreach (var preFile in request.requestDTO.ImagePaths)
                {
                    tempHall.Images.Add(new ImageData() {Name=preFile, ImageUri = preFile, HallId = hall.Id });
                }
                await _unitOfWork.HallRepository.UpdateAsync(tempHall);
                await _unitOfWork.HallRepository.SaveChangesAsync();

            }catch (Exception ex)
            {
                return false;
            }

            



            return true;
        }
    }
}
