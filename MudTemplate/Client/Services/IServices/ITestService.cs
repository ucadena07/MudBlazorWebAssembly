using MudTemplate.Shared.Models;

namespace MudTemplate.Client.Services.IServices
{
    public interface ITestService
    {
        Task<T> GetTest<T>();
    }
}
