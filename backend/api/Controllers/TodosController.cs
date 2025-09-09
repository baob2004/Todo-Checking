using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos;
using api.Entities;
using api.Mapping;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _ctx.TodoItems.Select(t => t.ToDto()).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var todo = await _ctx.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
            return todo == null ? NotFound() : Ok(todo.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = dto.ToEntity();

            await _ctx.TodoItems.AddAsync(entity);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TodoUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _ctx.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return NotFound();

            dto.UpdateEntity(entity);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var entity = await _ctx.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return NotFound();

            _ctx.Remove(entity);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}