using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class ChangeUserPasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(5)]
        public string NewPassword { get; set; }
    }
}
