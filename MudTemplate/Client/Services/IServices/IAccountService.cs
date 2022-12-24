using MudTemplate.Shared.Models;

namespace MudTemplate.Client.Services.IServices
{
    public interface IAccountService
    {
        Task<T> CreateUser<T>(User setObj);
        Task<T> Login<T>(AuthRequest authRequest);
    }
}
