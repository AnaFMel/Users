using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Insert the e-mail.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "The e-mail provided is invalid.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Insert the password.")]
        public string Password { get; set; } = null!;
    }
}
