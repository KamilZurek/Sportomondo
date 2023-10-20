namespace Sportomondo.Api.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
        public ICollection<User> Users { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, {Name}";
        }
    }
}
