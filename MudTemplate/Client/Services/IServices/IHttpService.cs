using MudTemplate.Shared.Models;

namespace MudTemplate.Client.Services.IServices
{
    public interface IHttpService
    {
        Task<T> SendAsync<T>(APIRequest apiRequest);

    }
}
