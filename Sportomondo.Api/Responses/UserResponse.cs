namespace Sportomondo.Api.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public string Role { get; set; }
        public int ActivitiesCount { get; set; }
        public int AchievementsPoints { get; set; }
    }
}
