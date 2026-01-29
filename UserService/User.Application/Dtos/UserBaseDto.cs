namespace User.Application.Dtos
{
    public class UserBaseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int Mobile { get; set; }
    }
}
