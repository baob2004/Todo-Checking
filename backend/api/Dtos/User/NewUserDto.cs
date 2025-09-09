using System.ComponentModel.DataAnnotations;

namespace api.Dtos.User
{
    public class NewUserDto
    {
        [Required]
        [StringLength(20)]
        public string Username { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}