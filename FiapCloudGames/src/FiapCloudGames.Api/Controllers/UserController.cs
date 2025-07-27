using System.Security.Claims;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtém um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Dados do usuário.</returns>
        /// <response code="200">Usuário encontrado.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(string id)
        {
            var user = _service.ObterPorId(id);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                Role = user.Role.ToString()
            });
        }

        /// <summary>
        /// Obtém o perfil do usuário autenticado.
        /// </summary>
        /// <returns>Dados do usuário autenticado.</returns>
        /// <response code="200">Perfil encontrado.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = _service.ObterPorId(userId);
            if (user == null) return NotFound();

            return Ok(new
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="user">Dados do usuário a ser criado.</param>
        /// <returns>Usuário criado.</returns>
        /// <response code="201">Usuário criado com sucesso.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 201)]
        public IActionResult CriarUser([FromBody] User user)
        {
            _service.Criar(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }
    }
}
