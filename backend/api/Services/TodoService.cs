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
using api.Models.Common;
using api.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public async Task<PagedResult<TodoDto>> GetTodosPagedAsync(
            string userId, PagedRequest req, bool? isDone, string? title, CancellationToken ct = default)
        {
            var q = _uow.TodoRepository.QueryByUser(userId);

            if (isDone.HasValue)
                q = q.Where(t => t.IsDone == isDone.Value);

            if (!string.IsNullOrWhiteSpace(title))
            {
                q = q.Where(t => t.Title.Contains(title));
            }
            // Tổng trước khi phân trang
            var total = await q.CountAsync(ct);

            // Thứ tự ổn định (mặc định theo Id)
            q = q.OrderBy(t => t.Id);

            // Phân trang + ProjectTo để DB chỉ trả cột cần thiết
            var items = await q
                .Skip((Math.Max(1, req.PageNumber) - 1) * req.PageSize)
                .Take(req.PageSize)
                .ProjectTo<TodoDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return new PagedResult<TodoDto>(items, total, req.PageNumber, req.PageSize);
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