using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthenticationTokenBackend.Data;
using AuthenticationTokenBackend.Models;
using AuthenticationTokenBackend.Models.DTOs;
using System.Security.Claims;

namespace AuthenticationTokenBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }
            return int.Parse(userIdClaim.Value);
        }

        // GET: api/Tareas
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<TareaDto>>>> GetTareas()
        {
            try
            {
                var userId = GetCurrentUserId();

                var tareas = await _context.Tareas
                    .Where(t => t.UsuarioId == userId)
                    .OrderByDescending(t => t.FechaCreacion)
                    .Select(t => new TareaDto
                    {
                        Id = t.Id,
                        Titulo = t.Titulo,
                        Descripcion = t.Descripcion,
                        Completada = t.Completada,
                        FechaCreacion = t.FechaCreacion,
                        FechaActualizacion = t.FechaActualizacion
                    })
                    .ToListAsync();

                return Ok(ApiResponseDto<List<TareaDto>>.SuccessResponse(tareas, "Tareas obtenidas exitosamente"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponseDto<List<TareaDto>>.ErrorResponse("No autorizado"));
            }
        }

        // GET: api/Tareas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<TareaDto>>> GetTarea(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var tarea = await _context.Tareas
                    .Where(t => t.Id == id && t.UsuarioId == userId)
                    .Select(t => new TareaDto
                    {
                        Id = t.Id,
                        Titulo = t.Titulo,
                        Descripcion = t.Descripcion,
                        Completada = t.Completada,
                        FechaCreacion = t.FechaCreacion,
                        FechaActualizacion = t.FechaActualizacion
                    })
                    .FirstOrDefaultAsync();

                if (tarea == null)
                {
                    return NotFound(ApiResponseDto<TareaDto>.ErrorResponse("Tarea no encontrada"));
                }

                return Ok(ApiResponseDto<TareaDto>.SuccessResponse(tarea, "Tarea obtenida exitosamente"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponseDto<TareaDto>.ErrorResponse("No autorizado"));
            }
        }

        // POST: api/Tareas
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<TareaDto>>> CreateTarea([FromBody] TareaDto tareaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponseDto<TareaDto>.ErrorResponse("Datos de entrada inválidos", errors));
                }

                var userId = GetCurrentUserId();

                var tarea = new Tarea
                {
                    Titulo = tareaDto.Titulo,
                    Descripcion = tareaDto.Descripcion,
                    Completada = tareaDto.Completada,
                    UsuarioId = userId,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();

                tareaDto.Id = tarea.Id;
                tareaDto.FechaCreacion = tarea.FechaCreacion;

                return CreatedAtAction(
                    nameof(GetTarea),
                    new { id = tarea.Id },
                    ApiResponseDto<TareaDto>.SuccessResponse(tareaDto, "Tarea creada exitosamente")
                );
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponseDto<TareaDto>.ErrorResponse("No autorizado"));
            }
        }

        // PUT: api/Tareas/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<TareaDto>>> UpdateTarea(int id, [FromBody] TareaDto tareaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponseDto<TareaDto>.ErrorResponse("Datos de entrada inválidos", errors));
                }

                var userId = GetCurrentUserId();

                var tarea = await _context.Tareas
                    .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == userId);

                if (tarea == null)
                {
                    return NotFound(ApiResponseDto<TareaDto>.ErrorResponse("Tarea no encontrada"));
                }

                tarea.Titulo = tareaDto.Titulo;
                tarea.Descripcion = tareaDto.Descripcion;
                tarea.Completada = tareaDto.Completada;
                tarea.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                tareaDto.Id = tarea.Id;
                tareaDto.FechaCreacion = tarea.FechaCreacion;
                tareaDto.FechaActualizacion = tarea.FechaActualizacion;

                return Ok(ApiResponseDto<TareaDto>.SuccessResponse(tareaDto, "Tarea actualizada exitosamente"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponseDto<TareaDto>.ErrorResponse("No autorizado"));
            }
        }

        // DELETE: api/Tareas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteTarea(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var tarea = await _context.Tareas
                    .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == userId);

                if (tarea == null)
                {
                    return NotFound(ApiResponseDto<object>.ErrorResponse("Tarea no encontrada"));
                }

                _context.Tareas.Remove(tarea);
                await _context.SaveChangesAsync();

                return Ok(ApiResponseDto<object?>.SuccessResponse(null, "Tarea eliminada exitosamente"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("No autorizado"));
            }
        }
    }
}
