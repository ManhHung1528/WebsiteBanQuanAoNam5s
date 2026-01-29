using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating;
using System.Drawing;
using Website.Areas.Admin.Attributes;
using Website.Models;
using Website.Models;
//sử dụng thư viện sau để phân trang
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;
using BC = BCrypt.Net;


namespace Website.Controllers
{
    public class ProductsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Category(int? id, int? page)
        {
            //xác định số trang hiện tại
            int page_number = page ?? 1;
            //số bản ghi trên một trang
            int page_size = 8;
            ViewBag.CategoryId = id;
            //lấy danh sách các bản ghi
            // Sai về xem lại
            //List<Products> listRecord = db.Products.ToList();
            List<Products> listRecord = (from p in db.Products
                                         join cp in db.categoriesProducts
                                             on p.Id equals cp.ProductId
                                         where cp.CategoryId == id
                                         select p).ToList();
            // Tạo 1 request để chọn
            string sapxep = HttpContext.Request.Query["sapxep"].ToString();
            switch (sapxep)
            {
                // Kết quả của biển listRecord có thể tiếp tục truy vấn
                // if chọn priceAsc ứng với price trong View thì listProduct sẽ sắp xếp
                case "priceAsc":
                    listRecord = listRecord.OrderBy(item => (item.Price - item.Discount)).ToList();
                    break;
                case "priceDesc":
                    listRecord = listRecord.OrderByDescending(item => (item.Price - item.Discount)).ToList();
                    break;
                case "nameAsc":
                    listRecord = listRecord.OrderBy(item => (item.Price - item.Discount)).ToList();
                    break;
                case "nameDesc":
                    listRecord = listRecord.OrderByDescending(item => (item.Price - item.Discount)).ToList();
                    break;
            }
            return View("ProductsCategory", listRecord.ToPagedList(page_number, page_size));
        }
        //chi tiết sản phẩm
        public IActionResult Detail(int id)
        {
            //lấy một bản ghi
            Products record = db.Products.FirstOrDefault(item => item.Id == id);
            var colors = db.Color.ToList();
            var sizes = db.Size.ToList();
            ViewBag.Color = colors;
            ViewBag.Size = sizes;
            return View("ProductDetail", record);
        }
        //đánh giá số sao của sản phẩm
        public IActionResult Rate(int id)
        {
            //lấy biến star truyền từ url
            int _Star = !String.IsNullOrEmpty(Request.Query["star"]) ? Convert.ToInt32(Request.Query["star"]) : 0;
            //thêm bản ghi vào table Rating
            Rating record = new Rating();
            record.ProductId = id;
            record.Star = _Star;
            db.Ratings.Add(record);
            db.SaveChanges();
            return Redirect("/Products/Detail/" + id);
        }
        public IActionResult ProductBuy(int id,int? SizeId, int? ColorId)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            var colors = db.Color.ToList();
            var sizes = db.Size.ToList();
            ViewBag.Color = colors;
            ViewBag.Size = sizes;
            if (SizeId.HasValue) HttpContext.Session.SetInt32("Ship_Size", SizeId.Value); 
            if (ColorId.HasValue) HttpContext.Session.SetInt32("Ship_Color", ColorId.Value);
            return View(product);
        }
        [HttpPost]
        public IActionResult ProductBuy(int ProductId,int SizeId,int ColorId,string FullName,string Phone,string Email,string Address,string City,string Ward,string Note,string Payment,int ShippingFee)
        {
            var colors = db.Color.ToList();
            var sizes = db.Size.ToList();
            ViewBag.Color = colors;
            ViewBag.Size = sizes;
            //HttpContext.Session.Remove("cart");
            //HttpContext.Session.Remove("Shipper");
            //HttpContext.Session.Remove("Ship_Payment");
            // 1. Thêm sản phẩm vào giỏ        
            // 2. Lưu thông tin giao hàng vào SESSION
            HttpContext.Session.SetString("Ship_FullName", FullName??"");
            HttpContext.Session.SetString("Ship_Phone", Phone ?? "");
            HttpContext.Session.SetString("Ship_Email", Email ?? "");
            HttpContext.Session.SetString("Ship_Address", Address ?? "");
            HttpContext.Session.SetString("Ship_City", City ?? "" );
            HttpContext.Session.SetString("Ship_Ward", Ward ?? "");
            HttpContext.Session.SetString("Ship_Note", Note ?? "");
            HttpContext.Session.SetString("Ship_Payment", Payment ?? "");
            HttpContext.Session.SetInt32("Shipper", ShippingFee);
            //  Lưu thêm Size và Color vào Session hoặc Cart
            HttpContext.Session.SetInt32("Ship_Size", SizeId); 
            HttpContext.Session.SetInt32("Ship_Color", ColorId);
            // 3. Chuyển sang Cart
            //ViewBag.Payment = Payment; 
            //var product = db.Products.FirstOrDefault(p => p.Id == ProductId); 
            //if(!string.IsNullOrEmpty(Payment))
            //{
            //    return View(product);
            //}
            if ((Payment == "COD" || Payment == "BANK") && !String.IsNullOrEmpty(FullName) && !String.IsNullOrEmpty(Phone) && !String.IsNullOrEmpty(Address))
            {
                Cart.CartAdd(HttpContext.Session, ProductId,SizeId, ColorId);
                return RedirectToAction("DatHangThanhCong", new { id = ProductId, sizeId = SizeId, colorId =ColorId});
            } else 
            {
                return RedirectToAction("ProductBuy", new { id = ProductId });

            }  
        }
        // Tìm kiếm sản phẩm theo tên
        public IActionResult Search(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return View(new List<Products>());

            var result = db.Products
                .Where(p => p.Name.ToLower().Contains(key.ToLower()))
                .ToList();

            ViewBag.Key = key;
            return View(result);
        }
        public IActionResult DatHangThanhCong(int id, int? sizeId, int? colorId)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            // Đổi tên biến để không trùng với tham số
            var sessionSizeId = HttpContext.Session.GetInt32("Ship_Size"); 
            var sessionColorId = HttpContext.Session.GetInt32("Ship_Color"); 
            var size = (sizeId ?? sessionSizeId) != null ? db.Size.FirstOrDefault(s => s.Id == (sizeId ?? sessionSizeId).Value) : null; 
            var color = (colorId ?? sessionColorId) != null ? db.Color.FirstOrDefault(c => c.Id == (colorId ?? sessionColorId).Value) : null; 
            ViewBag.SizeName = size?.Name ?? "Không chọn"; 
            ViewBag.ColorName = color?.Name ?? "Không chọn";
            return View(product); // View DatHangThanhCong.cshtml dùng @model Products
        }

    }
}
