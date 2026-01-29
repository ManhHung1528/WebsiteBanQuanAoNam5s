using Microsoft.AspNetCore.Mvc;
using Website.Areas.Admin.Attributes;
using Website.Models;
using X.PagedList;
using BC = BCrypt;
// Sử dụng thư viện sau để phân trang
using X.PagedList;
namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class NewsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return RedirectToAction("Read");
        }
        public IActionResult Read(int? Page)
        {
            // Xác định số trang hiện tại
            int page_number = Page ?? 1;
            // Số bản ghi trên một trang
            int page_size = 4;
            // Gọi List News tạo trong Model ứng bằng với News trong db và sắp xếp theo thứ tự giảm dần (Cái mới lên đầu)
            List<News> listNews = db.News.OrderByDescending(item => item.Id).ToList();
            // ToPagedList để phân trang
            return View("Read", listNews.ToPagedList(page_number, page_size));
        }
        
        public IActionResult Create()
        {
            ViewBag.formAction = "/Admin/News/CreatePost";
            return View("CreateUpdate");
        }
        public IActionResult CreatePost(IFormCollection fc)
        {
            // Lấy dữ liệu từ form
            string name = fc["name"];
            string _Description = fc["Description"];
            string _Content = fc["Content"];
            double _Price = Convert.ToDouble(fc["Price"]);
            double _Discount = Convert.ToDouble(fc["Discount"]);
            int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;

            // Khởi tạo đối tượng News
            News record = new News();
            record.Name = name;
            record.Description = _Description;
            record.Content = _Content;
            record.Hot = _Hot;

            // Upload ảnh
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    // Lấy tên file
                    string _Photo = Request.Form.Files[0].FileName;
                    // Tạo path lưu file
                    string _Path = Path.Combine("wwwroot/Upload/News", _Photo);

                    // Upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }

                    record.Photo = _Photo;
                }
            }
            catch (Exception ex)
            {
                ViewBag.error = "Upload ảnh thất bại: " + ex.Message;
                // Trả về form tạo lại (nếu muốn bạn có thể return View)
            }

            // Lưu vào database
            db.News.Add(record);
            db.SaveChanges();

            return Redirect("/Admin/News");
        }

        public IActionResult Update(int id)
        {
            // Lấy 1 bản ghi tương ứng với id truyền vào
            News rowNews = db.News.FirstOrDefault(item => item.Id == id);
            // Tạo biến formAction để lưu action của form (Không được sai cú pháp bắt buộc formAction)
            ViewBag.formAction = "/Admin/News/UpdatePost/" + id;
            return View("CreateUpdate", rowNews);
        }
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            // C1: Lấy giá trị của form controller theo IFormCollection
            string name = fc["name"]; // Phải name thường không hoa
            // C2: Lấy giá trị của form controller theo đối tượng Request
            string _Description = fc["Description"];
            string _Content = fc["Content"];
            int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;
            // Update bản ghi 
            News row = db.News.FirstOrDefault(item => item.Id == id);
            if (row != null)
            {
                row.Name = name;
                row.Description = _Description;
                row.Content = _Content;
                row.Hot = _Hot;
                // Upload ảnh
                try
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        // Lấy tên file
                        string _Photo = Request.Form.Files[0].FileName;
                        string _Path = Path.Combine("wwwroot/Upload/News", _Photo);
                        // Upload file
                        using (var stream = new FileStream(_Path, FileMode.Create))
                        {
                            Request.Form.Files[0].CopyTo(stream);
                        }
                        row.Photo = _Photo;
                    }
                    db.Update(row);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.error = "Thêm ảnh thất bại: " + ex.Message;
                }

            }
            return Redirect("/Admin/News");
        }
        public IActionResult Delete(int id)
        {
            News DeleteNews = db.News.Find(id);

            if (DeleteNews != null)
            {
                db.News.Remove(DeleteNews);
                db.SaveChanges();
            }

            return RedirectToAction("Read");
        }
    }
}
