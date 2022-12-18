using MudTemplate.Shared.Dtos;
using MudTemplate.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Shared.IRepositories
{
    public interface IUserRepository
    {
        Task<UserDto> Get(int id);
        Task<List<UserDto>> GetAll();
    }
}
