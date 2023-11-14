using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class ActivityRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
        [Required]
        public DateTime DateFinish { get; set; }
        [Required]
        public decimal Distance { get; set; }
        [Required]
        public int AverageHr { get; set; }
        [Required]
        [MaxLength(50)]
        public string City { get; set; }
        public string RouteUrl { get; set; }
    }
}
