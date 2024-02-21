namespace Sportomondo.Api.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public TimeSpan Time { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, {Type} - {Time}, Enabled: {Enabled}";
        }
    }
}
