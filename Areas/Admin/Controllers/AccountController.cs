using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;

using Website.Models;
using BC = BCrypt.Net;

namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private MyDbContext db = new MyDbContext();
        public IActionResult Login()
        {
            return View();
        }
        //[HttpGet]
        //public IActionResult TestUser()
        //{
        //    var user = db.Users.FirstOrDefault(x => x.Email == "admin@example.com");
        //    if (user == null) return Content("User not found");
        //    return Content("User exists: " + user.Email);
        //}

        //[HttpGet]
        //public IActionResult AddAdmin()
        //{
        //    if (!db.Users.Any(u => u.Email == "admin@example.com"))
        //    {
        //        var user = new Users
        //        {
        //            Name = "Admin",
        //            Email = "admin@example.com",
        //            Password = BC.BCrypt.HashPassword("123456")
        //        };
        //        db.Users.Add(user);
        //        db.SaveChanges();
        //    }

        //    return Content("Admin added!");
        //}
        [HttpPost]
        public IActionResult LoginPost(IFormCollection formCollection)
        {
            string email = formCollection["Email"].ToString();
            string password = formCollection["Password"].ToString();

            // Lấy bản ghi tương ứng 1 user chuyển vào
            Users user = db.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                // di chuyển đến url /Admin/Account/Login
                return Redirect("/Admin/Account/Login?notify=fail");
            }
            else
            {
                // kiểm tra password
                if (BC.BCrypt.Verify(password, user.Password))
                {
                    HttpContext.Session.SetString("Admin_Email", user.Email);
                    HttpContext.Session.SetString("Admin_Id", user.Id.ToString());
                    //return RedirectToAction("Index", "Home", new { area = "Admin" });
                    //password = BCrypt.Net.BCrypt.HashPassword("123456");
                    // ✅ Gán Role vào Session để CheckRoleAttribute dùng
                    HttpContext.Session.SetString("Role", user.Role);
                    return Redirect("/Admin/Home");
                    //return Ok("ok");
                }
            }
            return Redirect("/Admin/Account/Login?notify=fail");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();  // Đăng xuất khỏi cookie
            //return RedirectToAction("Login", "Account"); // Chạy vào Url: Account/Login
            return Redirect("/Admin/Account/Login");
        }
    }
}