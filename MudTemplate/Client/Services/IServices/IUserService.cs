using MudTemplate.Shared.Entities;

namespace MudTemplate.Client.Services.IServices
{
    public interface IUserService
    {
        Task<T> Get<T>(int id);
        Task<T> GetAll<T>();
    }
}
