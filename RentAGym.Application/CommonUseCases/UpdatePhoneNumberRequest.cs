using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentAGym.Application.CommonUseCases
{
    public sealed record UpdatePhoneNumberRequest(string UserId, string PhoneNumber) : IRequest<bool> 
    {
    }
}
