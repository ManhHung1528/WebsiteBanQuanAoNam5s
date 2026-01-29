using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
// Bắt buộc
using Newtonsoft.Json;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Website.Areas.Admin.Attributes;
using Website.Models;
using Website.Models;
using X.PagedList;
namespace Website.Controllers
{
    public class CartController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            // Lấy giỏ hàng từ session
            var cart = Cart.GetCart(HttpContext.Session);

            // Chống null
            if (cart == null)
                cart = new List<Item>();
            foreach (var item in cart)
            {
                item.SizeName = db.Size.FirstOrDefault(s => s.Id == item.SizeId)?.Name;
                item.ColorName = db.Color.FirstOrDefault(c => c.Id == item.ColorId)?.Name;
            }
            // Trả về view Index.cshtml với model là List<Item>
            return View("Index", cart);
        }

        // Cho sản phẩm vào giỏ hàng
        [HttpPost] // thêm dòng này để cho phép POST
        public IActionResult Buy(int id, int sizeId, int colorId, string FullName, string Phone, string Address, string City, string Ward, string Note, string Payment, int ShippingFee)
        {
            // Thêm sản phẩm vào giỏ
            Cart.CartAdd(HttpContext.Session, id, sizeId, colorId);

            // Lưu địa chỉ vào Session
            HttpContext.Session.SetString("Ship_FullName", FullName ?? "");
            HttpContext.Session.SetString("Ship_Phone", Phone ?? "");
            HttpContext.Session.SetString("Ship_Address", Address ?? "");
            HttpContext.Session.SetString("Ship_City", City ?? "");
            HttpContext.Session.SetString("Ship_Ward", Ward ?? "");
            HttpContext.Session.SetString("Ship_Note", Note ?? "");
            HttpContext.Session.SetString("Ship_Payment", Payment ?? "");
            HttpContext.Session.SetInt32("Shipper", ShippingFee);
            return RedirectToAction("Index", "Cart");
        }
        
        public IActionResult Buy(int id, int sizeId, int colorId)
        {
            //gọi hàm Add từ class Cart
            Cart.CartAdd(HttpContext.Session, id, sizeId, colorId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult BuyCart(int ProductId, int SizeId, int ColorId)
        {
            Cart.CartAdd(HttpContext.Session, ProductId, SizeId, ColorId);
            return RedirectToAction("Index");
        }
        // Xoá sản phẩm khỏi giỏ hàng
        public IActionResult Remove(int id)
        {
            // Gọi hàm Remove từ class Cart
            Cart.CartRemove(HttpContext.Session, id);
            return RedirectToAction("Index");
        }
        // Cập nhật số lượng sản phẩm
        [HttpPost]
        public IActionResult Update()
        {
            // Lấy chuỗi json 
            string json_cart = HttpContext.Session.GetString("cart");
            // Tạo biến Cart để đổ dữ liệu từ biến json vào 
            List<Item> cart = new List<Item>();
            if (!String.IsNullOrEmpty(json_cart))
            {
                // Chuyển json ra dạng list
                cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
            }
            // Duyệt các Item trong list cart để Update số lượng
            foreach (var product in cart)
            {
                int quantity = Convert.ToInt32(Request.Form["product_" + product.ProductRecord.Id]);
                // Gọi hàm Cart Update để Update số lượng
                Cart.CartUpdate(HttpContext.Session, product.ProductRecord.Id, quantity);
            }
            return Redirect("/Cart");
        }
        // Xoá toàn bộ sản phẩm trong giỏ hàng
        public IActionResult Destroy(int id)
        {
            Cart.CartDestroy(HttpContext.Session);
            return Redirect("/Cart");
        }
        // Thanh toán giỏ hàng
        public IActionResult Checkout()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("customer_user_email")))
            {
                Cart.CartCheckOut(HttpContext.Session, Convert.ToInt32(HttpContext.Session.GetString("customer_user_id")));
                return Redirect("/Cart");
            }
            else

                return Redirect("/Account/Login");

            return Redirect("/Cart");
        }


    }
}
