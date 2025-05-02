using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentAGym.Application.CommonUseCases
{
    public sealed class UpdatePhoneNumberHandler : IRequestHandler<UpdatePhoneNumberRequest, bool>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdatePhoneNumberHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> Handle(UpdatePhoneNumberRequest request, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return false;

            var setPhoneResult = await userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
            if (!setPhoneResult.Succeeded)
                return false;

            return true;
        }
    }
}
