using BCrypt.Net;

namespace FiapCloudGames.Domain.Entities
public class User
{
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, Password);
    
    }   
}