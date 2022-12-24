using Microsoft.EntityFrameworkCore;
using MudTemplate.Backend;
using MudTemplate.Shared.Entities;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Models;
using MudTemplate.Shared.Utilities.IUtilities;

namespace MudTemplate.Backend.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        public AccountRepository(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
    
        }
        public async Task CreateUser(User model)
        {


            var hashs = _passwordService.CreatePasswordHash(model.Password);
            SiteUser newSiteUser = new()
            {
                UserName = model.UserName,
                Active = true,
                Blocked = false,
                PasswordHash = hashs.passwordHash,
                PasswordSalt = hashs.passwordSalt,

            };

            await _context.SiteUsers.AddAsync(newSiteUser);
            await _context.SaveChangesAsync();
        }

    }
}
