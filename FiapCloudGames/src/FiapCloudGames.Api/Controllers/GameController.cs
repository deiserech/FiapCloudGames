using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FiapCloudGames.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de jogos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _service;

        public GameController(IGameService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Cadastra um novo jogo
        /// </summary>
        /// <remarks>
        /// Apenas administradores podem cadastrar jogos.
        /// </remarks>
        /// <param name="game">Dados do jogo a ser cadastrado</param>
        /// <returns>O jogo cadastrado</returns>
        /// <response code="201">Jogo cadastrado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="403">Acesso negado - apenas administradores</response>
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult Cadastrar([FromBody] Game game)
        {
            if (game == null)
                return BadRequest("Game data is required");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _service.Cadastrar(game);
            return CreatedAtAction(nameof(ObterPorId), new { id = game.Id }, game);
        }

        /// <summary>
        /// Obtém um jogo pelo ID
        /// </summary>
        /// <param name="id">ID do jogo</param>
        /// <returns>O jogo encontrado</returns>
        /// <response code="200">Jogo encontrado</response>
        /// <response code="404">Jogo não encontrado</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ObterPorId(int id)
        {
            var game = _service.ObterPorId(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        /// <summary>
        /// Lista todos os jogos cadastrados
        /// </summary>
        /// <returns>Lista de jogos</returns>
        /// <response code="200">Lista de jogos retornada com sucesso</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<Game>), StatusCodes.Status200OK)]
        public IActionResult ListarTodos()
        {
            return Ok(_service.ListarTodos());
        }
    }


}
