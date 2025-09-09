using System.ComponentModel.DataAnnotations;

namespace api.Dtos
{
    public class TodoCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = default!;
        public bool IsDone { get; set; } = false;
        public DateTime? Due { get; set; }
    }
}