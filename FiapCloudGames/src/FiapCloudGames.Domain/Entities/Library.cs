using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Domain.Entities
{
    public class Library
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do usuário deve ser um número positivo.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "O ID do jogo é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do jogo deve ser um número positivo.")]
        public int GameId { get; set; }

        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}

