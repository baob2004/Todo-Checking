using System.ComponentModel.DataAnnotations;

namespace api.Dtos.User
{
    public class LoginDto
    {
        [Required]
        [StringLength(20)]
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}