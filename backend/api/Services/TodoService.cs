using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using api.Dtos;
using api.Entities;
using api.Interfaces.Common;
using api.Interfaces.Services;
using api.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class TodoService : ITodoService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public TodoService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<TodoDto> CreateTodoAsync(string userId, TodoCreateDto dto)
        {
            var entity = _mapper.Map<TodoItem>(dto);
            entity.UserId = userId;

            await _uow.TodoRepository.AddAsync(entity);
            await _uow.SaveChangesAsync();

            // lúc này entity.Id đã có giá trị
            var res = _mapper.Map<TodoDto>(entity);
            return res;
        }

        public async Task<bool> DeleteTodoAsync(string userId, int id)
        {
            var todo = await _uow.TodoRepository.GetAsync(t => t.UserId == userId && t.Id == id);
            if (todo == null) return false;

            _uow.TodoRepository.Remove(todo);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<TodoDto?> GetTodoAsync(string userId, int id)
        {
            var todo = await _uow.TodoRepository.GetAsync(t => t.UserId == userId && t.Id == id);
            var res = _mapper.Map<TodoDto>(todo);
            return res;
        }

        public async Task<List<TodoDto>> GetTodosAsync(string userId, bool? isDone)
        {
            Expression<Func<TodoItem, bool>> predicate = t => t.UserId == userId;

            if (isDone.HasValue)
            {
                predicate = t => t.UserId == userId && t.IsDone == isDone.Value;
            }

            var todos = await _uow.TodoRepository.GetAllAsync(predicate);

            var res = _mapper.Map<List<TodoDto>>(todos);

            return res;
        }

        public async Task<TodoDto?> UpdateTodoAsync(string userId, int id, TodoUpdateDto dto)
        {
            var todo = await _uow.TodoRepository.GetAsync(t => t.UserId == userId && t.Id == id);
            if (todo == null) return null;

            // cập nhật field cho phép sửa
            _mapper.Map(dto, todo);
            _uow.TodoRepository.Update(todo);

            await _uow.SaveChangesAsync();

            var res = _mapper.Map<TodoDto>(todo);
            return res;
        }
    }
}