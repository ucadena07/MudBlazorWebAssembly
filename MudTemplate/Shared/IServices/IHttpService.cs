using MudTemplate.Shared.Models;

namespace MudTemplate.Shared.IServices
{
    public interface IHttpService
    {
        Task<T> SendAsync<T>(APIRequest apiRequest);

    }
}
