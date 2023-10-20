using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Sportomondo.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<Activity> Activities { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, {FirstName} {LastName}";
        }
    }
}
