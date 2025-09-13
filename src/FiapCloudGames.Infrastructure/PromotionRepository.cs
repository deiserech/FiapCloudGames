using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Domain.Interfaces.Repositories;

namespace FiapCloudGames.Infrastructure
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDbContext _context;

        public PromotionRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Promotion?> GetByIdAsync(int id)
        {
            return await _context.Promotions
                .Include(p => p.Game)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Promotions
                .Include(p => p.Game)
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsByGameIdAsync(int gameId)
        {
            var now = DateTime.UtcNow;
            return await _context.Promotions
                .Include(p => p.Game)
                .Where(p => p.GameId == gameId && p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .ToListAsync();
        }

        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            
            return await GetByIdAsync(promotion.Id) ?? promotion;
        }

        public async Task<Promotion> UpdateAsync(Promotion promotion)
        {
            _context.Entry(promotion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return await GetByIdAsync(promotion.Id) ?? promotion;
        }

        public async Task DeleteAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }
        }

    }
}
