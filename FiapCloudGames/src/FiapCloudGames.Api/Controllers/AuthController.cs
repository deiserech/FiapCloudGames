using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.DTOs;
using FiapCloudGames.Domain.Utils;
using System;
using FiapCloudGames.Domain.Interfaces.Services;

namespace FiapCloudGames.Api.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação e registro de usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Realiza o login de um usuário no sistema
        /// </summary>
        /// <param name="loginDto">Dados de login (email e senha)</param>
        /// <returns>Token JWT e informações do usuário autenticado</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="401">Email ou senha inválidos</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="registerDto">Dados para registro do usuário</param>
        /// <returns>Token JWT e informações do usuário registrado</returns>
        /// <response code="200">Usuário registrado com sucesso</response>
        /// <response code="400">Dados inválidos, senha não atende aos critérios ou email já está em uso</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var passwordErrors = ValidationHelper.ValidatePassword(registerDto.Password);
            if (passwordErrors.Any())
            {
                foreach (var error in passwordErrors)
                {
                    ModelState.AddModelError("Password", error);
                }
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
