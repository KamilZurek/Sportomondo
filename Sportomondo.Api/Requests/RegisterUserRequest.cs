using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class RegisterUserRequest //validation in RegisterUserRequestValidator
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
