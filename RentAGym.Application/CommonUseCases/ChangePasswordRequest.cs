using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentAGym.Application.CommonUseCases
{
    public sealed record ChangePasswordRequest(string UserId, [Required] string OldPassword, [Required] string NewPassword, [Required] string ConfirmPassword)
        : IRequest<(bool Success, string? Error)>
    { 
    }
}
