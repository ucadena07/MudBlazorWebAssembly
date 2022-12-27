using MudTemplate.Shared.Models;

namespace MudTemplate.Shared.IServices
{
    public interface IAccountService
    {
        Task<T> CreateUser<T>(User setObj);
        Task<T> Login<T>(AuthRequest authRequest);
        Task<T> RenewToken<T>(RefreshTokenRequest refreshRequest);
    }
}
