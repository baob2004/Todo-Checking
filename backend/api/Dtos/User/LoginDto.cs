using System.ComponentModel.DataAnnotations;

namespace api.Dtos.User
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}