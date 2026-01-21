namespace User.Domain.Models
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Mobile { get; set; }
    }
}
