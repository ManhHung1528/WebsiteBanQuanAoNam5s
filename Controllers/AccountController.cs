using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Website.Models;
using BC = BCrypt.Net;
namespace Website.Controllers
{
    public class AccountController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        public IActionResult RegisterPost(IFormCollection fc)
        {
            string _Name = fc["name"];
            string _Email = fc["email"];
            string _Address = fc["address"];
            string _Phone = fc["phone"];
            string _Password = fc["password"];
            _Password = BC.BCrypt.HashPassword(_Password);
            Customers record = new Customers();
            record.Name = _Name;
            record.Email = _Email;
            record.Address = _Address;
            record.Phone = _Phone;
            record.Password = _Password;
            db.Customers.Add(record);
            db.SaveChanges();
            return Redirect("/Account/Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginPost(IFormCollection formCollection)
        {
            string _email = formCollection["email"].ToString();
            string _password = formCollection["password"].ToString();
            //lấy một bản ghi tương ứng với user truyền vào
            Customers user = db.Customers.FirstOrDefault(x => x.Email == _email);
            if (user == null)
            {
                //di chuyển đến url /Admin/Account/Login
                return Redirect("/Account/Login?notify=fail");
            }
            else
            {
                //kiểm tra password
                if (BC.BCrypt.Verify(_password, user.Password))
                {
                    //đăng nhập thành công, khởi tạo các session
                    HttpContext.Session.SetString("customer_user_email", user.Email);
                    HttpContext.Session.SetString("customer_user_id", user.Id.ToString());
                    //di chuyển đến url /Admin/Home
                    return Redirect("/Home");
                }
            }
            return Redirect("/Home?notify=fail");
        }
        //Đăng xuất
        public IActionResult Logout()
        {
            //hủy các biến session
            HttpContext.Session.Remove("customer_user_email");
            HttpContext.Session.Remove("customer_user_id");
            return RedirectToAction("Login", "Account");
        }
    }
}
