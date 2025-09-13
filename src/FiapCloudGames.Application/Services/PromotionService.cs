using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FiapCloudGames.Domain.Interfaces.Services;

namespace FiapCloudGames.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IGameRepository _gameRepository;

        private const int MaximumPromotionDurationDays = 30;
        private const int MaxActivePromotionsPerGame = 3;

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

            await ValidatePromotionBusinessRules(promotion);

            return await _promotionRepository.CreateAsync(promotion);
        }

        public async Task<Promotion> UpdatePromotionAsync(Promotion promotion)
        {
            var existing = await _promotionRepository.GetByIdAsync(promotion.Id);
            if (existing == null)
            {
                throw new ArgumentException("Promoção não encontrada.");
            }

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

            await ValidatePromotionBusinessRules(promotion, promotion.Id);

            existing.Title = promotion.Title;
            existing.Description = promotion.Description;
            existing.DiscountPercentage = promotion.DiscountPercentage;
            existing.DiscountAmount = promotion.DiscountAmount;
            existing.StartDate = promotion.StartDate;
            existing.EndDate = promotion.EndDate;
            existing.IsActive = promotion.IsActive;
            existing.GameId = promotion.GameId;
            return await _promotionRepository.UpdateAsync(existing);
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

        public async Task<decimal> GetDiscountedPriceAsync(int gameId)
        {
            var bestPromotion = await GetBestPromotionForGameAsync(gameId);


            return bestPromotion?.CalculateDiscountedPrice()??0;
        }

        public async Task<Promotion?> GetBestPromotionForGameAsync(int gameId)
        {
            var activePromotions = await GetActivePromotionsByGameIdAsync(gameId);

            if (!activePromotions.Any())
            {
                return null;
            }

            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return null;
            }

            var originalPrice = game.Price;

            return activePromotions
                .OrderBy(p => p.CalculateDiscountedPrice())
                .FirstOrDefault();
        }

        private async Task ValidatePromotionBusinessRules(Promotion promotion, int? excludePromotionId = null)
        {
            ValidatePromotionDuration(promotion);

            ValidatePromotionStartDate(promotion);

            await ValidateActivePromotionsLimit(promotion.GameId, excludePromotionId);
        }

        private static void ValidatePromotionDuration(Promotion promotion)
        {
            var duration = promotion.EndDate - promotion.StartDate;
            var durationDays = (int)duration.TotalDays;

            if (durationDays > MaximumPromotionDurationDays)
            {
                throw new ArgumentException($"A duração da promoção não pode exceder {MaximumPromotionDurationDays} dias.");
            }
        }


        private static void ValidatePromotionStartDate(Promotion promotion)
        {
            var today = DateTime.Today;
            if (promotion.StartDate.Date < today)
            {
                throw new ArgumentException("A data de início da promoção não pode ser no passado.");
            }
        }

        private async Task ValidateActivePromotionsLimit(int gameId, int? excludePromotionId)
        {
            var activePromotions = await GetActivePromotionsByGameIdAsync(gameId);

             var activeCount = activePromotions.Count();

            if (activeCount >= MaxActivePromotionsPerGame)
            {
                throw new InvalidOperationException($"O jogo já possui o limite máximo de {MaxActivePromotionsPerGame} promoção(ões) ativa(s). Desative uma promoção existente antes de criar uma nova.");
            }
        }
    }
}
