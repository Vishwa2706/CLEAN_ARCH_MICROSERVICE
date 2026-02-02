namespace IdentityService.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public long Mobile { get; set; }
        public string Role { get; set; } = "Member";
        public string PasswordHash { get; set; } = null!;
    }
}
