using MudTemplate.Shared.Entities;

namespace MudTemplate.Shared.IServices
{
    public interface IUserService
    {
        Task<T> Get<T>(int id);
        Task<T> GetAll<T>();
    }
}
