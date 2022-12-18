using Microsoft.Extensions.DependencyInjection;
using MudTemplate.Backend.Repositories;
using MudTemplate.Shared.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MudTemplate.Backend.Helpers;
using MudTemplate.Shared.Utilities.IUtilities;
using MudTemplate.Shared.Utilities;

namespace MudTemplate.Backend
{
    public static class IServiceCollectionMudBlazorTemplate
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {

            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(new[] { typeof(MappingProfile).Assembly });

            return services;
        }
    }
}
