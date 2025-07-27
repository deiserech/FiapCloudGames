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

        [HttpGet("{id}")]
        public IActionResult GetUser(string id)
        {
            var user = _service.ObterPorId(id);
            if (user == null) return NotFound();

            // Remove senha do retorno
            return Ok(new
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }

        [HttpGet("profile")]
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
                Email = user.Email
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CriarUser([FromBody] User user)
        {
            _service.Criar(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }
    }
}
