using System.ComponentModel.DataAnnotations;

namespace Users.API.DTOs
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "The user's ID is required.")]
        public int Id { get; set; }
        public string? Name { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "The e-mail provided is invalid.")]
        public string? Email { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "The password provided is invalid. It needs to have at least 8 digits, containing of numbers, letters and special caracters.")]
        public string? Password { get; set; }
        public int? RoleId { get; set; }
    }
}
