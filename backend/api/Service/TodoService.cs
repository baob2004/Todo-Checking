using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Interfaces.Common;
using api.Interfaces.Services;
using api.Mapping;
using api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Service
{
    public class TodoService : ITodoService
    {
        private readonly IUnitOfWork _uow;
        public TodoService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TodoDto> CreateTodoAsync(string userId, TodoCreateDto dto)
        {
            var entity = dto.ToEntity();
            entity.UserId = userId;

            await _uow.TodoRepository.AddAsync(entity);
            await _uow.SaveChangesAsync();

            return entity.ToDto();
        }

        public async Task<bool> DeleteTodoAsync(string userId, int id)
        {
            var q = _uow.TodoRepository.QueryByUser(userId);
            var entity = await q.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return false;

            _uow.TodoRepository.Remove(entity);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<TodoDto?> GetTodoAsync(string userId, int id)
        {
            var query = _uow.TodoRepository.QueryByUser(userId);
            var todo = await query.Where(t => t.UserId == userId).FirstOrDefaultAsync(t => t.Id == id);
            return todo == null ? null : todo.ToDto();
        }

        public async Task<List<TodoDto>> GetTodosAsync(string userId, bool? isDone)
        {
            var query = _uow.TodoRepository.QueryByUser(userId);

            if (isDone.HasValue) query = query.Where(t => t.IsDone == isDone);

            var todos = await query.Select(t => t.ToDto()).ToListAsync();

            return todos;
        }

        public async Task<TodoDto?> UpdateTodoAsync(string userId, int id, TodoUpdateDto dto)
        {
            var q = _uow.TodoRepository.QueryByUser(userId);
            var entity = await q.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return null;

            // cập nhật field cho phép sửa
            dto.UpdateEntity(entity);

            _uow.TodoRepository.Update(entity);
            await _uow.SaveChangesAsync();

            return entity.ToDto();
        }
    }
}