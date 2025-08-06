namespace FiapCloudGames.Domain.Entities
{
    public class Library
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GameId { get; set; }

        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
    }
}
