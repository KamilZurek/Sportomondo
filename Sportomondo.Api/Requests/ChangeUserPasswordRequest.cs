using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class ChangeUserPasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
