using System.Security.Claims;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Obt�m um usu�rio pelo ID.
        /// </summary>
        /// <param name="id">ID do usu�rio.</param>
        /// <returns>Dados do usu�rio.</returns>
        /// <response code="200">Usu�rio encontrado.</response>
        /// <response code="404">Usu�rio n�o encontrado.</response>
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
        /// Obt�m o perfil do usu�rio autenticado.
        /// </summary>
        /// <returns>Dados do usu�rio autenticado.</returns>
        /// <response code="200">Perfil encontrado.</response>
        /// <response code="401">Usu�rio n�o autenticado.</response>
        /// <response code="404">Usu�rio n�o encontrado.</response>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

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
        /// Cria um novo usu�rio.
        /// </summary>
        /// <param name="user">Dados do usu�rio a ser criado.</param>
        /// <returns>Usu�rio criado.</returns>
        /// <response code="201">Usu�rio criado com sucesso.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 201)]
        public IActionResult CriarUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is required");

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
