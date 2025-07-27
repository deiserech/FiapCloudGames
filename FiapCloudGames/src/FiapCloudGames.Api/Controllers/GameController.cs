using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameService _service;

        public GameController(GameService service)
        {
            _service = service;
        }

        /// <summary>
        /// Cadastra um novo jogo.
        /// </summary>
        /// <remarks>
        /// Apenas administradores podem cadastrar jogos.
        /// </remarks>
        /// <param name="game">Dados do jogo a ser cadastrado.</param>
        /// <returns>O jogo cadastrado.</returns>
        /// <response code="201">Jogo cadastrado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(Game), 201)]
        [ProducesResponseType(400)]
        public IActionResult Cadastrar([FromBody] Game game)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _service.Cadastrar(game);
            return CreatedAtAction(nameof(ObterPorId), new { id = game.Id }, game);
        }

        /// <summary>
        /// Obtém um jogo pelo ID.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        /// <returns>O jogo encontrado.</returns>
        /// <response code="200">Jogo encontrado.</response>
        /// <response code="404">Jogo não encontrado.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Game), 200)]
        [ProducesResponseType(404)]
        public IActionResult ObterPorId(int id)
        {
            var game = _service.ObterPorId(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        /// <summary>
        /// Lista todos os jogos cadastrados.
        /// </summary>
        /// <returns>Lista de jogos.</returns>
        /// <response code="200">Lista de jogos retornada com sucesso.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Game>), 200)]
        public IActionResult ListarTodos()
        {
            return Ok(_service.ListarTodos());
        }
    }


}
