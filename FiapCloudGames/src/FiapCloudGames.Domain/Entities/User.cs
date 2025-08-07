using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Domain.Enums;

namespace FiapCloudGames.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [StringLength(255, ErrorMessage = "O e-mail deve ter no máximo 255 caracteres.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "O papel do usuário é obrigatório.")]
        public required UserRole Role { get; set; } = UserRole.User;

        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Library> LibraryGames { get; set; } = new List<Library>();

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

