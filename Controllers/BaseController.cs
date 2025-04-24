using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyTrungTam.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RestoreSessionFromCookies();
            base.OnActionExecuting(context);
        }

        protected void RestoreSessionFromCookies()
        {
            string username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                string cookieUsername = Request.Cookies["Username"];
                string roleStr = Request.Cookies["Role"];
                string studentIdStr = Request.Cookies["StudentId"];

                if (!string.IsNullOrEmpty(cookieUsername))
                {
                    HttpContext.Session.SetString("Username", cookieUsername);

                    if (int.TryParse(roleStr, out int role))
                        HttpContext.Session.SetInt32("Role", role);

                    if (int.TryParse(studentIdStr, out int studentId))
                        HttpContext.Session.SetInt32("StudentId", studentId);
                }
            }
        }
    }
}
