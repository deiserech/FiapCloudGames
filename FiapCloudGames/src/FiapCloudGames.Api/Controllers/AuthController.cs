using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.DTOs;

namespace FiapCloudGames.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _authService.Login(loginDto);
            
            if (result == null)
            {
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _authService.Register(registerDto);
            
            if (result == null)
            {
                return BadRequest(new { message = "Email já está em uso" });
            }

            return Ok(result);
        }
    }
}
