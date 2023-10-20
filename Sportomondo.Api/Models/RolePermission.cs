namespace Sportomondo.Api.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, RoleId: {RoleId}, {Name}: {Enabled}";
        }
    }
}
