using Sportomondo.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class CreateActivityRequest : ActivityRequest
    { 
        [Required]
        [Range(0, 2)]
        public ActivityType Type { get; set; }
    }
}
