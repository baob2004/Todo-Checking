using api.Dtos;
using api.Models.Common;

namespace api.Interfaces.Services
{
    public interface ITodoService
    {
        Task<PagedResult<TodoDto>> GetTodosPagedAsync(
             string userId, PagedRequest req, bool? isDone, string? title, CancellationToken ct = default);
        Task<TodoDto?> GetTodoAsync(string userId, int id);
        Task<TodoDto> CreateTodoAsync(string userId, TodoCreateDto dto);
        Task<TodoDto?> UpdateTodoAsync(string userId, int id, TodoUpdateDto dto);
        Task<bool> DeleteTodoAsync(string userId, int id);
    }
}