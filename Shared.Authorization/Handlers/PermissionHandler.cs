using Microsoft.AspNetCore.Authorization;
using Shared.Authorization.Requirements;

namespace Shared.Authorization.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement
        )
        {
            var permissions = context.User.FindAll("permission").Select(x => x.Value);

            if (permissions.Contains(requirement.Permission))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
