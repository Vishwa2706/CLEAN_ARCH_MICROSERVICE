using Microsoft.AspNetCore.Authorization;

namespace Shared.Authorization.Attributes
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public PermissionAuthorizeAttribute(string permission)
        {
            Policy = permission;
        }
    }
}

//creating a custom attribute using authorizeattribute
