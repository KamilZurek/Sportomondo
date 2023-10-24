using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class LoginUserRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
