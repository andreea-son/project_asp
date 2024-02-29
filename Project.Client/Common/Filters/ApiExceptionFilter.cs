using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project.Client.Common.Exceptions;

namespace Project.Client.Common.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        // check if the exception is an ApiException
        if (context.Exception is ApiException apiException)
        {
            // handle unauthorized errors by prompting the user to re-login
            if (apiException.Error?.Status == 401)
            {
                SignOutSynchronously(context.HttpContext);
                var currentUrl = context.HttpContext.Request.GetDisplayUrl();
                context.HttpContext.Response.Redirect(currentUrl); // force an entire refresh of the current location, so that header and footer are re-rendered too
            }
            else
            {
                string? error = apiException.Error?.Errors?.First();
                string? validationError = apiException.Error?.ValidationErrors?.First().Value.First();
                // return a JSON result with the error details
                context.Result = new JsonResult(new
                {
                    success = false,
                    errorMessage = apiException.Message + (!string.IsNullOrEmpty(error) ? Environment.NewLine + error : string.Empty) +
                        (!string.IsNullOrEmpty(validationError) ? Environment.NewLine + validationError : string.Empty)
                });  
            }
            // mark the exception as handled to prevent propagation
            context.ExceptionHandled = true;
        }
        else
        {
            if (context.Exception.Message == "authentication error!")
            {
                SignOutSynchronously(context.HttpContext);
                var currentUrl = context.HttpContext.Request.GetDisplayUrl();
                context.HttpContext.Response.Redirect(currentUrl); // force an entire refresh of the current location, so that header and footer are re-rendered too
                context.ExceptionHandled = true;
            }
        }
    }

    /// <summary>
    /// Signs out the user synchronously by deleting the authentication cookie and token.
    /// </summary>
    /// <param name="httpContext">The HttpContext for the current request.</param>
    private static void SignOutSynchronously(HttpContext httpContext)
    {
        var task = httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        task.Wait();  // run the task synchronously
        httpContext.Response.Cookies.Delete("Token");
    }
}
