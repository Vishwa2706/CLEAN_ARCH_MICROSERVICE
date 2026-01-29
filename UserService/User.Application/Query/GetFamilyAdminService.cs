using User.Application.Contracts;
using User.Application.Dtos;

namespace User.Application.Query
{
    public class GetFamilyAdminService
    {
        private readonly IUserService _userService;

        public GetFamilyAdminService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserBaseDto?> Execute(int userId)
        {
            var user = await _userService.GetUserPermissions(userId);

            if (user == null)
                throw new ArgumentException("User not found");

            if (user.Role != "Admin")
                throw new ArgumentException("User does not have admin access");

            return new AdminUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Mobile = user.Mobile,
                Permissions = new List<string>
                {
                    "ADD_EXPENSE",
                    "EDIT_EXPENSE",
                    "ADD_MEMBER"
                }
            };
        }
    }
}
