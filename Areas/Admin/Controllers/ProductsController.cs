using Microsoft.AspNetCore.Mvc;
// Lấy CheckLogin Đăng nhập vào mới xử lý được dữ liệu
using Website.Areas.Admin.Attributes;
// Sử dụng Model
using Website.Models;
// Sử dụng thư viện sau để phân trang
using X.PagedList;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    [CheckRole("Admin")]
    public class ProductsController : Controller
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
            // Gọi List Products tạo trong Model ứng bằng với Products trong db và sắp xếp theo thứ tự giảm dần (Cái mới lên đầu)
            List<Products> listproducts = db.Products.OrderByDescending(item => item.Id).ToList();
            // ToPagedList để phân trang
            return View("Read", listproducts.ToPagedList(page_number, page_size));
        }
        public void CreateUpdateCategoriesProducts(int _ProductId)
        {
            //lấy giá trị của biến form có name=Categories
            List<string> categories = Request.Form["Categories"].ToList();
            //xóa hết các bản ghi tương ứng với _ProductId
            List<CategoriesProduct> list_categories_products = db.categoriesProducts.Where(item => item.ProductId == _ProductId).ToList();
            foreach (var item in list_categories_products)
            {
                db.categoriesProducts.Remove(item);
                db.SaveChanges();
            }
            //---
            foreach (string category in categories)
            {
                int _CategoryId = Convert.ToInt32(category);
                //thêm mới bản ghi vào table CategoriesProducts
                CategoriesProduct record = new CategoriesProduct();
                record.ProductId = _ProductId;
                record.CategoryId = _CategoryId;
                db.categoriesProducts.Add(record);
                db.SaveChanges();
            }
        }
        public IActionResult Create()
        {
            ViewBag.formAction = "/Admin/Products/CreatePost";
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

            // Khởi tạo đối tượng Products
            Products record = new Products();
            record.Name = name;
            record.Description = _Description;
            record.Content = _Content;
            record.Price = _Price;
            record.Discount = _Discount;
            record.Hot = _Hot;

            // Upload ảnh
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    // Lấy tên file
                    string _Photo = Request.Form.Files[0].FileName;
                    // Tạo path lưu file
                    string _Path = Path.Combine("wwwroot/Upload/Products", _Photo);

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
            db.Products.Add(record);
            db.SaveChanges();

            return Redirect("/Admin/Products");
        }

        public IActionResult Update(int id)
        {
            // Lấy 1 bản ghi tương ứng với id truyền vào
            Products rowProducts = db.Products.FirstOrDefault(item => item.Id == id);
            // Tạo biến formAction để lưu action của form (Không được sai cú pháp bắt buộc formAction)
            ViewBag.formAction = "/Admin/Products/UpdatePost/" + id;
            return View("CreateUpdate", rowProducts);
        }
        public IActionResult UpdatePost(int id, IFormCollection fc)
        {
            // C1: Lấy giá trị của form controller theo IFormCollection
            string name = fc["name"]; // Phải name thường không hoa
            // C2: Lấy giá trị của form controller theo đối tượng Request
            string _Description = fc["Description"];
            string _Content = fc["Content"];
            double _Price = Convert.ToDouble(fc["Price"]);
            double _Discount = Convert.ToDouble(fc["Discount"]);
            int _Hot = !String.IsNullOrEmpty(fc["Hot"]) ? 1 : 0;
            // Update bản ghi 
            Products row = db.Products.FirstOrDefault(item => item.Id == id);
            if (row != null)
            {
                row.Name = name;
                row.Description = _Description;
                row.Content = _Content;
                row.Price = _Price;
                row.Discount = _Discount;
                row.Hot = _Hot;
                // Upload ảnh
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files[0];

                    if (file != null && file.Length > 0)
                    {
                        // 1. Xóa ảnh cũ
                        if (!string.IsNullOrEmpty(row.Photo))
                        {
                            string oldPath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot/Upload/Products",
                                row.Photo
                            );

                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // 2. Upload ảnh mới
                        string newPhoto = Path.GetFileName(file.FileName);
                        string newPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/Upload/Products",
                            newPhoto
                        );

                        using (var stream = new FileStream(newPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // 3. Cập nhật tên ảnh mới vào DB
                        row.Photo = newPhoto;
                    }
                    db.Update(row);
                    db.SaveChanges();
                }

            }
            CreateUpdateCategoriesProducts(id);
            return Redirect("/Admin/Products");
        }
        public IActionResult Delete(int id)
        {
            Products deleteProduct = db.Products.Find(id);

            if (deleteProduct != null)
            {
                db.Products.Remove(deleteProduct);
                db.SaveChanges();
            }

            return RedirectToAction("Read");
        }
    }
}
