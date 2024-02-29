using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Project.Client.Common.Filters;

public class UserTokenActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items["UserToken"] = context.HttpContext.User?.FindFirst("Token")?.Value;
        context.HttpContext.Items["UserRole"] = context.HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // no implementation needed here
    }
}