using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required UserRole Role { get; set; } = UserRole.Usuario;
        public string PasswordHash { get; set; } = string.Empty;

        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(PasswordHash))
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
