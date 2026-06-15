using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Insert the user's name.")]
        public string Name { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "The e-mail provided is invalid.")]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "The password provided is invalid. It needs to have at least 8 digits, containing of numbers, letters and special caracters.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Insert the user's role.")]
        public int RoleId { get; set; }
    }
}
