using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Website.Areas.Admin.Attributes
{
    public class CheckRoleAttribute : ActionFilterAttribute
    {
        private readonly string _role;
        public CheckRoleAttribute(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetString("Role");
            if (userRole != _role)
            {
                // Redirect về Action AccessDenied trong HomeController của Area Admin
                context.Result = new RedirectToActionResult(
                    "AccessDenied",   // Action
                    "Home",           // Controller
                    new { area = "Admin" } // Area
                );
            }
        }

    }
}
