using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class TodoCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = default!;
        public bool IsDone { get; set; }
        public DateTime? Due { get; set; }
    }
}