using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Backend.Helpers.Interfaces
{
    public interface IGlobalSettings
    {
        Task<string> GetCurrentUserId();
        Task<string> GetJwtKey();
        Task<string> GetApplicationId();
    }
}
