using System.Security.Claims;
using api.Data;
using api.Dtos;
using api.Interfaces.Services;
using api.Mapping;
using api.Service;
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
        public TodosController(AppDbContext ctx, ITodoService todoSvc)
        {
            _ctx = ctx;
            _todoSvc = todoSvc;
        }
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isDone)
        {
            var userId = GetUserId();
            var todos = await _todoSvc.GetTodosAsync(userId, isDone);

            return Ok(todos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userId = GetUserId();
            var todo = await _todoSvc.GetTodoAsync(userId, id);

            return todo == null ? NotFound() : Ok(todo);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserId();
            var createdTodo = await _todoSvc.CreateTodoAsync(userId, dto);

            return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TodoUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserId();
            var entity = await _todoSvc.UpdateTodoAsync(userId, id, dto);
            if (entity == null) return NotFound();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetUserId();
            var exists = await _todoSvc.DeleteTodoAsync(userId, id);
            if (exists is false) return NotFound();

            return NoContent();
        }
    }
}