using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Sportomondo.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        //public ROLA? Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}
