using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentAGym.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RentAGym.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("DefaultConnection");
            services.AddPooledDbContextFactory<ApplicationDbContext>(opt =>
            {
                opt.UseMySql(connString, ServerVersion.AutoDetect(connString));
            });
            services.AddDbContext<ApplicationDbContext>(opt => {
                opt.UseMySql(connString, ServerVersion.AutoDetect(connString));
            });
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
