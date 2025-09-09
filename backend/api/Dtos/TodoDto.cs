using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class TodoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public bool IsDone { get; set; }
        public DateTime? Due { get; set; }
    }
}