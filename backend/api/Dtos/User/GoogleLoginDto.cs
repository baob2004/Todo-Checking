using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class GoogleLoginDto
    {
        [Required]
        public string IdToken { get; set; } = default!;
    }
}