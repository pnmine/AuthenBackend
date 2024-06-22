using AuthenBackend.DTOs.Auth;

namespace AuthenBackend.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<ServiceResponse<AuthResponseDto>> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}