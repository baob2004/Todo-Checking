using api.Dtos;

namespace api.Interfaces.Services
{
    public interface ITodoService
    {
        public Task<List<TodoDto>> GetTodosAsync(string userId, bool? isDone);
        public Task<TodoDto?> GetTodoAsync(string userId, int id);
        Task<TodoDto> CreateTodoAsync(string userId, TodoCreateDto dto);
        Task<TodoDto?> UpdateTodoAsync(string userId, int id, TodoUpdateDto dto);
        Task<bool> DeleteTodoAsync(string userId, int id);
    }
}