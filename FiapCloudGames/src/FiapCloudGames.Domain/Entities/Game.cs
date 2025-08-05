using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        // Navigation Property para Promotions
        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

        // Navigation Property para Library
        public ICollection<Library> LibraryEntries { get; set; } = new List<Library>();
    }
}
