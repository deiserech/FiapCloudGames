    using Microsoft.EntityFrameworkCore;
    using FiapCloudGames.Domain.Entities;
    
    namespace FiapCloudGames.Infrastructure.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
            public DbSet<User> Users { get; set; }
        }
    }