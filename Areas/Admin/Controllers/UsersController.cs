using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
// Lấy Attribute để dùng checkLogin
using Website.Areas.Admin.Attributes;
// Gọi Model Users
using Website.Models;
// Gọi using dùng X.Page
using X.PagedList;
// Gọi using dùng Bc
using Bc = BCrypt.Net;
namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    [CheckRole("Admin")]
    public class UsersController : Controller
    {
        // Gọi Db để lấy Users
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return RedirectToAction("Read");
        }
        // Hiển thị bản ghi Read
        public IActionResult Read(int? Page)
        {
            // Xác định số trang muốn tạo là bao nhiêu
            int page_Number = Page ?? 1;// Nếu Page có giá trị → gán page_Number = Page, không có =1
            // Số bản ghi trong 1 trang
            int page_Size = 4;
            // Dùng list lấy danh sách bản ghi
            // Sắp xếp danh sách người dùng mới nhất lên đầu (Id lớn nhất trước).
            List<Users> list_Users = db.Users.OrderByDescending(item => item.Id).ToList();
            // Hiển thị Read, Lấy danh sách user hiển thị ra Views
            // Chỉ lấy các bản ghi tương ứng với trang page_Number,
            // Mỗi trang chứa page_Size mục
            return View("Read",list_Users.ToPagedList(page_Number,page_Size));
        }
        public IActionResult Create()
        {
            ViewBag.formAction = "/Admin/Users/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {

            string name = fc["name"]; // Phải name thường không hoa
            // Lấy giá trị của form controller theo đối tượng Request
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            // Nghiên cứu thêm: Nếu enail đã tồn 
            // Tạo bản ghi để chuẩn bị Insert

            string role = Request.Form["role"]; // lấy role từ form
            Users row = new Users();
            row.Name = name;
            row.Email = email;
            row.Password = Bc.BCrypt.HashPassword(password);
            row.Role = role; // gán role
            db.Users.Add(row);
            db.SaveChanges();
            return Redirect("/Admin/Users");
        }
        public IActionResult Update(int id)
        {
            // Lấy 1 bản ghi tương ứng với id truyền vào
            Users rowUser = db.Users.FirstOrDefault(item => item.Id == id);
            // Tạo biến formAction để lưu action của form (Không được sai cú pháp bắt buộc formAction)
            ViewBag.formAction = "/Admin/Users/UpdatePost/" + id;
            return View("CreateUpdate", rowUser);
        }
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            // C1: Lấy giá trị của form controller theo IFormCollection
            string role = Request.Form["role"];
            string name = fc["name"]; // Phải name thường không hoa
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            Users rowUser = db.Users.FirstOrDefault(item => item.Id == id);
            rowUser.Name = name;
            rowUser.Email = email;
            if (!string.IsNullOrEmpty(password)) 
            {
                rowUser.Password = Bc.BCrypt.HashPassword(password);
            }           
            rowUser.Role = role;
            // Update
            db.Users.Update(rowUser);
            db.SaveChanges();
            // C2: Lấy giá trị của form controller theo đối tượng Request
            //string email = Request.Form["email"];
            //string password = Request.Form["password"];
            //// Update bản ghi 
            //Users row = db.Users.FirstOrDefault(item => item.Id == id);
            //if (row != null)
            //{
            //    row.Name = name;
            //    row.Email = email;
            //    // Nếu password không rỗng thì update password
            //    if (!String.IsNullOrEmpty(password))
            //    {
            //        row.Password = Bc.BCrypt.HashPassword(password);
            //    }
            //    db.Update(row);
            //    db.SaveChanges();
            //}
            return Redirect("/Admin/Users");
        }
        public IActionResult Delete(int id)
        {
            Users row = db.Users.Find(id);
            if (row != null)
            {
                db.Users.Remove(row);
                db.SaveChanges();
            }
            return Redirect("/Admin/Users");
        }
    }
}
