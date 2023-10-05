using System.ComponentModel.DataAnnotations;

namespace Sportomondo.Api.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateFinish { get; set; }
        public ActivityType Type { get; set; }
        public decimal Distance { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Pace { get; set; }
        public int AverageHr { get; set; }
        public string City { get; set; }
        public decimal KcalBurned { get; set; }
        public Weather Weather { get; set; } //co z id do pogody w db //czy moze istniec bez pogody?
        public string RouteUrl { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } // = null!;
        //czy pprop tylko GET tez doda do db?
    }
}
