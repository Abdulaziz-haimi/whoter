using System.Threading.Tasks;
//using WaterCollector.BackendApi.Contracts.Auth;

namespace WaterCollector.BackendApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
