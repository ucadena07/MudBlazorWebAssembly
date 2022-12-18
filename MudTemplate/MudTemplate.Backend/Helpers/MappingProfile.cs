using AutoMapper;
using MudTemplate.Shared.Dtos;
using MudTemplate.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Backend.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SiteUser, UserDto>().ReverseMap();
        }
    }
}
