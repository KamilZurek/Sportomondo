namespace Sportomondo.Api.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ActivityType ActivityType { get; set; }
        public CountingType CountingType { get; set; }
        public decimal CountingRequiredValue { get; set; }
        public int Points { get; set; }
        public ICollection<User> Users { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, {Name}";
        }
    }
}
