using MudTemplate.Shared.Models;

namespace MudTemplate.Client.Auth
{
    public interface ILoginService
    {
        Task Login(UserToken userToken);
        Task Logout();
        Task TryRenewToken();
    }
}
