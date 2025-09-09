using api.Dtos;
using api.Entities;

namespace api.Mapping
{
    public static class TodoMapping
    {
        public static TodoDto ToDto(this TodoItem entity)
        {
            return new TodoDto
            {
                Id = entity.Id,
                Title = entity.Title,
                IsDone = entity.IsDone,
                Due = entity.Due
            };
        }

        public static TodoItem ToEntity(this TodoCreateDto dto)
        {
            return new TodoItem
            {
                Title = dto.Title,
                IsDone = dto.IsDone,
                Due = dto.Due
            };
        }

        public static void UpdateEntity(this TodoUpdateDto dto, TodoItem entity)
        {
            entity.Title = dto.Title;
            entity.IsDone = dto.IsDone;
            entity.Due = dto.Due;
        }
    }
}