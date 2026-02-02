namespace User.Domain.Models
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public long Mobile { get; set; }

        public string Role { get; set; }

        public string PasswordHash { get; set; }

        //navigation property
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
