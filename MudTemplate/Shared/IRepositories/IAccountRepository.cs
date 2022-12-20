using MudTemplate.Shared.Entities;
using MudTemplate.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Shared.IRepositories
{
    public interface IAccountRepository
    {
        Task CreateUser(User model);
        Task<bool> VerifyUser(User model);
    }
}
