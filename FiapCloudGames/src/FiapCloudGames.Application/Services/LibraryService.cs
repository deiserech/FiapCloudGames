using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;

namespace FiapCloudGames.Application.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPromotionService _promotionService;

        public LibraryService(
            ILibraryRepository libraryRepository,
            IUserRepository userRepository,
            IGameRepository gameRepository,
            IPromotionService promotionService)
        {
            _libraryRepository = libraryRepository;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _promotionService = promotionService;
        }

        public async Task<IEnumerable<Library>> GetUserLibraryAsync(int userId)
        {
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            return await _libraryRepository.GetByUserIdAsync(userId);
        }

        public async Task<Library?> GetLibraryEntryAsync(int id)
        {
            return await _libraryRepository.GetByIdAsync(id);
        }

        public async Task<Library?> GetUserGameAsync(int userId, int gameId)
        {
            return await _libraryRepository.GetByUserAndGameAsync(userId, gameId);
        }

        public async Task<IEnumerable<Library>> GetRecentPurchasesAsync(int userId, int days = 30)
        {
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            return await _libraryRepository.GetRecentPurchasesAsync(userId, days);
        }

        public async Task<IEnumerable<Library>> GetGiftedGamesAsync(int userId)
        {
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            return await _libraryRepository.GetGiftedGamesAsync(userId);
        }

        public async Task<Library> PurchaseGameAsync(int userId, int gameId, decimal purchasePrice, bool isGift = false, string? giftMessage = null)
        {
            // Validações
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            if (!await _gameRepository.ExistsAsync(gameId))
            {
                throw new ArgumentException("Jogo não encontrado.");
            }

            if (await UserOwnsGameAsync(userId, gameId))
            {
                throw new InvalidOperationException("Usuário já possui este jogo em sua biblioteca.");
            }

            if (purchasePrice <= 0)
            {
                throw new ArgumentException("Preço de compra deve ser maior que zero.");
            }

            var library = new Library
            {
                UserId = userId,
                GameId = gameId,
                PurchaseDate = DateTime.Now,
                PurchasePrice = purchasePrice,
                IsGift = isGift,
                GiftMessage = giftMessage
            };

            return await _libraryRepository.CreateAsync(library);
        }

        public async Task<Library> GiftGameAsync(int fromUserId, int toUserId, int gameId, string? giftMessage = null)
        {
            // Validações
            if (!await _userRepository.ExistsAsync(fromUserId))
            {
                throw new ArgumentException("Usuário remetente não encontrado.");
            }

            if (!await _userRepository.ExistsAsync(toUserId))
            {
                throw new ArgumentException("Usuário destinatário não encontrado.");
            }

            if (fromUserId == toUserId)
            {
                throw new ArgumentException("Não é possível presentear a si mesmo.");
            }

            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new ArgumentException("Jogo não encontrado.");
            }

            if (await UserOwnsGameAsync(toUserId, gameId))
            {
                throw new InvalidOperationException("O usuário destinatário já possui este jogo.");
            }

            // Usa o preço com desconto se houver promoção ativa
            var currentPrice = await _promotionService.GetDiscountedPriceAsync(gameId, game.Price);

            return await PurchaseGameAsync(toUserId, gameId, currentPrice, true, giftMessage);
        }

        public async Task<bool> UserOwnsGameAsync(int userId, int gameId)
        {
            return await _libraryRepository.UserOwnsGameAsync(userId, gameId);
        }

        public async Task<int> GetTotalGamesCountAsync(int userId)
        {
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            return await _libraryRepository.GetTotalGamesCountByUserAsync(userId);
        }

        public async Task<decimal> GetTotalSpentAsync(int userId)
        {
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            return await _libraryRepository.GetTotalSpentByUserAsync(userId);
        }

        public async Task<decimal> GetTotalSavingsAsync(int userId)
        {
            var userLibrary = await GetUserLibraryAsync(userId);
            decimal totalSavings = 0;

            foreach (var entry in userLibrary)
            {
                if (entry.Game != null)
                {
                    totalSavings += entry.GetSavingsAmount(entry.Game.Price);
                }
            }

            return totalSavings;
        }

        public async Task<IEnumerable<Library>> GetPurchaseHistoryAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!await _userRepository.ExistsAsync(userId))
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                return await _libraryRepository.GetPurchasesByDateRangeAsync(userId, startDate.Value, endDate.Value);
            }

            return await _libraryRepository.GetByUserIdAsync(userId);
        }

        public async Task RemoveFromLibraryAsync(int userId, int gameId)
        {
            var libraryEntry = await _libraryRepository.GetByUserAndGameAsync(userId, gameId);
            if (libraryEntry == null)
            {
                throw new ArgumentException("Jogo não encontrado na biblioteca do usuário.");
            }

            await _libraryRepository.DeleteAsync(libraryEntry.Id);
        }

        public async Task<IEnumerable<Game>> GetRecommendedGamesAsync(int userId, int count = 10)
        {
            // Implementação básica de recomendação
            // Pode ser expandida com algoritmos mais sofisticados
            var userLibrary = await GetUserLibraryAsync(userId);
            var ownedGameIds = userLibrary.Select(l => l.GameId).ToList();

            // Para esta implementação básica, vamos recomendar jogos que o usuário não possui
            // Em uma implementação real, poderíamos usar machine learning ou análise de preferências
            var allGames = await _gameRepository.GetAllAsync();
            
            return allGames
                .Where(g => !ownedGameIds.Contains(g.Id))
                .Take(count);
        }
    }
}
