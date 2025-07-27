using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Domain.DTOs
{
    public class RegisterDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required UserRole Role { get; set; }
    }
}
