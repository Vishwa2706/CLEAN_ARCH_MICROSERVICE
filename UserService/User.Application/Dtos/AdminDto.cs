namespace User.Application.Dtos
{
    public class AdminUserDto : UserBaseDto
    {
        public List<string> Permissions { get; set; } = new();
    }
}
