using FiapCloudGames.Domain.DTOs;

namespace FiapCloudGames.Domain.Interfaces
{
    public interface IAuthService
    {
        AuthResponseDto? Login(LoginDto loginDto);
        AuthResponseDto? Register(RegisterDto registerDto);
    }
}
