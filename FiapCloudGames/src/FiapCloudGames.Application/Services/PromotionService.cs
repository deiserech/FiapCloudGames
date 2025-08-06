using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FiapCloudGames.Domain.Interfaces.Services;

namespace FiapCloudGames.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IGameRepository _gameRepository;

        public PromotionService(IPromotionRepository promotionRepository, IGameRepository gameRepository)
        {
            _promotionRepository = promotionRepository;
            _gameRepository = gameRepository;
        }

        public async Task<Promotion?> GetPromotionByIdAsync(int id)
        {
            return await _promotionRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            return await _promotionRepository.GetActivePromotionsAsync();
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsByGameIdAsync(int gameId)
        {
            return await _promotionRepository.GetActivePromotionsByGameIdAsync(gameId);
        }

        public async Task<Promotion> CreatePromotionAsync(Promotion promotion)
        {
            // Validações
            if (promotion.StartDate >= promotion.EndDate)
            {
                throw new ArgumentException("A data de início deve ser anterior à data de fim.");
            }

            if (!await _gameRepository.ExistsAsync(promotion.GameId))
            {
                throw new ArgumentException("O jogo especificado não existe.");
            }

            if (promotion.DiscountPercentage <= 0 && (!promotion.DiscountAmount.HasValue || promotion.DiscountAmount <= 0))
            {
                throw new ArgumentException("Deve ser especificado um desconto percentual ou um valor de desconto.");
            }

            return await _promotionRepository.CreateAsync(promotion);
        }

        public async Task<Promotion> UpdatePromotionAsync(Promotion promotion)
        {
            var existingPromotion = await _promotionRepository.GetByIdAsync(promotion.Id);
            if (existingPromotion == null)
            {
                throw new ArgumentException("Promoção não encontrada.");
            }

            // Validações
            if (promotion.StartDate >= promotion.EndDate)
            {
                throw new ArgumentException("A data de início deve ser anterior à data de fim.");
            }

            if (!await _gameRepository.ExistsAsync(promotion.GameId))
            {
                throw new ArgumentException("O jogo especificado não existe.");
            }

            if (promotion.DiscountPercentage <= 0 && (!promotion.DiscountAmount.HasValue || promotion.DiscountAmount <= 0))
            {
                throw new ArgumentException("Deve ser especificado um desconto percentual ou um valor de desconto.");
            }

            return await _promotionRepository.UpdateAsync(promotion);
        }

        public async Task DeletePromotionAsync(int id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id);
            if (promotion == null)
            {
                throw new ArgumentException("Promoção não encontrada.");
            }

            await _promotionRepository.DeleteAsync(id);
        }

        public async Task<decimal> GetDiscountedPriceAsync(int gameId, decimal originalPrice)
        {
            var bestPromotion = await GetBestPromotionForGameAsync(gameId);
            if (bestPromotion == null)
            {
                return originalPrice;
            }

            return bestPromotion.CalculateDiscountedPrice(originalPrice);
        }

        public async Task<Promotion?> GetBestPromotionForGameAsync(int gameId)
        {
            var activePromotions = await GetActivePromotionsByGameIdAsync(gameId);

            if (!activePromotions.Any())
            {
                return null;
            }

            // Para determinar a melhor promoção, vamos usar o jogo para obter o preço original
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return null;
            }

            var originalPrice = game.Price;

            // Encontra a promoção que oferece o maior desconto
            return activePromotions
                .OrderBy(p => p.CalculateDiscountedPrice(originalPrice))
                .FirstOrDefault();
        }
    }
}
