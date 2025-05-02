using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentAGym.Application.CommonUseCases
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordRequest, (bool Success, string? Error)>
    {
        private readonly IServiceProvider _serviceProvider;

        public ChangePasswordHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<(bool Success, string? Error)> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return (false, "Пароли не совпадают");

            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return (false, "Пользователь не найден");

            var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            await signInManager.RefreshSignInAsync(user);
            return (true, null);
        }
    }
}
