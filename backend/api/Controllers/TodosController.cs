using System.Security.Claims;
using api.Data;
using api.Dtos;
using api.Mapping;
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
        public TodosController(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isDone)
        {
            var userId = GetUserId();
            var query = _ctx.TodoItems.AsQueryable();

            if (isDone.HasValue) query = query.Where(t => t.IsDone == isDone);

            var todos = await query.Where(t => t.UserId == userId).Select(t => t.ToDto()).ToListAsync();
            return Ok(todos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userId = GetUserId();
            var todo = await _ctx.TodoItems.Where(t => t.UserId == userId).FirstOrDefaultAsync(t => t.Id == id);
            return todo == null ? NotFound() : Ok(todo.ToDto());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserId();
            var entity = dto.ToEntity();
            entity.UserId = userId;

            await _ctx.TodoItems.AddAsync(entity);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToDto());
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TodoUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserId();
            var entity = await _ctx.TodoItems.Where(t => t.UserId == userId).FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return NotFound();

            dto.UpdateEntity(entity);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = GetUserId();
            var entity = await _ctx.TodoItems.Where(t => t.UserId == userId).FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return NotFound();

            _ctx.Remove(entity);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}