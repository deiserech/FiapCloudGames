using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces.Repositories;
using FiapCloudGames.Domain.Interfaces.Services;

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

        public async Task<Library> PurchaseGameAsync(int userId, int gameId)
        {
            if (!await _userRepository.ExistsAsync(userId))
                throw new ArgumentException("Usuário não encontrado.");

            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
                throw new ArgumentException("Jogo não encontrado.");

            if (await UserOwnsGameAsync(userId, gameId))
                throw new InvalidOperationException("Usuário já possui este jogo em sua biblioteca.");

            var finalPrice = await _promotionService.GetDiscountedPriceAsync(gameId);

            if (finalPrice <= 0)
                throw new ArgumentException("Preço de compra deve ser maior que zero.");

            var library = new Library
            {
                UserId = userId,
                GameId = gameId,
            };

            return await _libraryRepository.CreateAsync(library);
        }

        public async Task<bool> UserOwnsGameAsync(int userId, int gameId)
        {
            return await _libraryRepository.UserOwnsGameAsync(userId, gameId);
        }
    }
}
