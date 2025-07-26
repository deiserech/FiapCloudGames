using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CriarUser([FromBody] User user)
        {
            _service.Criar(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
    }
}