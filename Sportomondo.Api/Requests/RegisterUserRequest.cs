using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class RegisterUserRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
