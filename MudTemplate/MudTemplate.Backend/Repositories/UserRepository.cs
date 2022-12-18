using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MudTemplate.Shared.Dtos;
using MudTemplate.Shared.Entities;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Utilities.IUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private IMapper _mapper;

        public UserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper= mapper;
        }
        public async Task<UserDto> Get(int id)
        {
            var data = await _context.SiteUsers.FirstOrDefaultAsync(it => it.UserId == id);
            return _mapper.Map<UserDto>(data);
        }

        public async Task<List<UserDto>> GetAll()
        {
            var data = await _context.SiteUsers.ToListAsync();
            return _mapper.Map<List<UserDto>>(data);
        }
    }
}
