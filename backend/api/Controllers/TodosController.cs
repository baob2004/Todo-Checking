using System.Security.Claims;
using api.Data;
using api.Dtos;
using api.Entities;
using api.Interfaces.Services;
using api.Models.Common;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly ITodoService _todoSvc;
        private readonly ILogger<TodosController> _logger;
        public TodosController(AppDbContext ctx, ITodoService todoSvc, ILogger<TodosController> logger)
        {
            _ctx = ctx;
            _todoSvc = todoSvc;
            _logger = logger;
        }
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isDone, [FromQuery] string? title, [FromQuery] PagedRequest pagedRequest)
        {
            try
            {
                var userId = GetUserId();
                var todos = await _todoSvc.GetTodosPagedAsync(userId, pagedRequest, isDone, title);

                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetAll)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later");
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var userId = GetUserId();
                var todo = await _todoSvc.GetTodoAsync(userId, id);

                return todo == null ? NotFound() : Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetById)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later");
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var createdTodo = await _todoSvc.CreateTodoAsync(userId, dto);

                return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Create)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later");
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TodoUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var userId = GetUserId();
                var entity = await _todoSvc.UpdateTodoAsync(userId, id, dto);
                if (entity == null) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Update)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later");
            }

        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var userId = GetUserId();
                var exists = await _todoSvc.DeleteTodoAsync(userId, id);
                if (exists is false) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Delete)}");
                return StatusCode(500, "Internal Server Error. Please Try Again Later");
            }
        }
    }
}