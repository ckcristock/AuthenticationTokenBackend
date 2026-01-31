using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthenticationTokenBackend.Data;
using AuthenticationTokenBackend.Models;
using AuthenticationTokenBackend.Models.DTOs;
using AuthenticationTokenBackend.Services;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationTokenBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponseDto<LoginResponseDto>.ErrorResponse("Datos de entrada inválidos", errors));
            }

            // Hash de la contraseña
            var hashedPassword = HashPassword(loginRequest.Contrasena);

            // Buscar usuario
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == loginRequest.Usuario && u.Contrasena == hashedPassword);

            if (user == null)
            {
                return Unauthorized(ApiResponseDto<LoginResponseDto>.ErrorResponse("Usuario o contraseña incorrectos"));
            }

            // Generar token
            var token = _tokenService.GenerateToken(user.Id, user.Usuario);
            var expirationDate = DateTime.UtcNow.AddHours(24);

            // Actualizar token y fecha de último login
            user.Token = token;
            user.FechaUltimoLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                Id = user.Id,
                Usuario = user.Usuario,
                Token = token,
                FechaExpiracion = expirationDate
            };

            return Ok(ApiResponseDto<LoginResponseDto>.SuccessResponse(response, "Login exitoso"));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Register([FromBody] LoginRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponseDto<LoginResponseDto>.ErrorResponse("Datos de entrada inválidos", errors));
            }

            // Verificar si el usuario ya existe
            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == registerRequest.Usuario);

            if (existingUser != null)
            {
                return BadRequest(ApiResponseDto<LoginResponseDto>.ErrorResponse("El usuario ya existe"));
            }

            // Hash de la contraseña
            var hashedPassword = HashPassword(registerRequest.Contrasena);

            // Crear nuevo usuario
            var newUser = new User
            {
                Usuario = registerRequest.Usuario,
                Contrasena = hashedPassword,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            // Generar token
            var token = _tokenService.GenerateToken(newUser.Id, newUser.Usuario);
            var expirationDate = DateTime.UtcNow.AddHours(24);

            // Actualizar token
            newUser.Token = token;
            await _context.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                Id = newUser.Id,
                Usuario = newUser.Usuario,
                Token = token,
                FechaExpiracion = expirationDate
            };

            return Ok(ApiResponseDto<LoginResponseDto>.SuccessResponse(response, "Usuario registrado exitosamente"));
        }

        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<ApiResponseDto<object>>> Logout()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Token inválido"));
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await _context.Usuarios.FindAsync(userId);

            if (user != null)
            {
                user.Token = null;
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponseDto<object?>.SuccessResponse(null, "Sesión cerrada exitosamente"));
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
