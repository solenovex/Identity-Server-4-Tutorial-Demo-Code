using System.Linq;
using System.Threading.Tasks;
using HybridClient.Requirements;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;

namespace HybridClient.Auths
{
    public class SmithInSomewhereHandler : AuthorizationHandler<SmithInSomewareRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            SmithInSomewareRequirement requirement)
        {
            //var filterContext = context.Resource as AuthorizationFilterContext;
            //if (filterContext == null)
            //{
            //    context.Fail();
            //    return Task.CompletedTask;
            //}

            var familyName = context.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?.Value;
            var location = context.User.Claims.FirstOrDefault(c => c.Type == "location")?.Value;

            if (familyName == "Smith" && location == "somewhere" && context.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;

            // 一个Handler成功，其它的Handler没有失败 => Requirement被满足了
            // 某个Hanlder失败 => 无法满足Requirement
            // 没有成功和失败 => 无法满足Requirement
        }
    }
}
