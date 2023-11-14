using Sportomondo.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class CreateAchievementRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Range(0,2)]
        public ActivityType ActivityType { get; set; }
        [Required]
        [Range(0, 1)]
        public CountingType CountingType { get; set; }
        [Required]
        public decimal CountingRequiredValue { get; set; }
        [Required]
        public int Points { get; set; }
    }
}
