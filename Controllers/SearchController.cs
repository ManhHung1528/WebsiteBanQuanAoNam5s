using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Linq;
using System.Security.Cryptography;
using Website.Areas.Admin.Attributes;
using Website.Models;
using Website.Models;
//sử dụng thư viện sau để phân trang
using X.PagedList;
using BC = BCrypt.Net;

namespace project.Controllers
{
    public class SearchController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SearchPrice(int id, int? page)
        {
            //xác định số trang hiện tại
            int page_number = page ?? 1;
            //số bản ghi trên một trang
            int page_size = 8;
            string fromPrice = HttpContext.Request.Query["fromPrice"];
            string toPrice = HttpContext.Request.Query["toPrice"];
            ViewBag.fromPrice = fromPrice;
            ViewBag.toPrice = toPrice;
            List<Products> listRecord = db.Products.OrderByDescending(item => (item.Price- item.Discount)).ToList();

            if (!String.IsNullOrEmpty(toPrice) && listRecord.Count > 0)
            {
                listRecord = listRecord.Where(item => (item.Price - item.Discount) <= Convert.ToDouble(toPrice)).ToList();
            }
            if (!String.IsNullOrEmpty(fromPrice) && listRecord.Count > 0)
            {
                listRecord = listRecord.Where(item => (item.Price - item.Discount) >= Convert.ToDouble(fromPrice)).ToList();
            }
            return View("SearchPrice", listRecord.ToPagedList(page_number, page_size));
        }
        public IActionResult SearchName(int id, int? page)
        {
            //xác định số trang hiện tại
            int page_number = page ?? 1;
            //số bản ghi trên một trang
            int page_size = 8;
            string key = HttpContext.Request.Query["key"];
            ViewBag.key = key;
            List<Products> listRecord = db.Products.OrderByDescending(item => item.Id).ToList();
            if (!String.IsNullOrEmpty(key) && listRecord.Count > 0)
            {
                listRecord = listRecord.Where(item => item.Name.Contains(key)).ToList();
            }

            return View("SearchName", listRecord.ToPagedList(page_number, page_size));
        }
    }
}

