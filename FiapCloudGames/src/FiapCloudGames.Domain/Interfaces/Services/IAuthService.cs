using FiapCloudGames.Domain.DTOs;

namespace FiapCloudGames.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        AuthResponseDto? Login(LoginDto loginDto);
        AuthResponseDto? Register(RegisterDto registerDto);
    }
}
