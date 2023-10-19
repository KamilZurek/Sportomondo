using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Requests
{
    public class ActivityRequest //trzeba eksrea walidacje do wiekszosci pól
    {
        [Required]
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
        public string City { get; set; }
        public string RouteUrl { get; set; }
    }
}
