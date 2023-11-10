using Sportomondo.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class CreateAchievementRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public ActivityType ActivityType { get; set; }
        [Required]
        public CountingType CountingType { get; set; }
        [Required]
        public decimal CountingRequiredValue { get; set; }
        [Required]
        public int Points { get; set; }
    }
}
