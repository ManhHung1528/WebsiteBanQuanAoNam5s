using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Website.Areas.Admin.Attributes;
using Website.Models;
using System.Security.Cryptography;
using X.PagedList;
using Website.Areas.Admin.Controllers;

namespace Website.Controllers
{
    public class NewsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        //hiển thị danh sách các bản ghi
        public IActionResult Index(int? page)
        {
            //xác định số trang hiện tại
            int page_number = page ?? 1;
            //số bản ghi trên một trang
            int page_size = 4;
            //lấy danh sách các bản ghi
            List<News> listRecord = db.News.OrderByDescending(item => item.Id).ToList();
            return View("Index", listRecord.ToPagedList(page_number, page_size));
        }
        public IActionResult Detail(int id)
        {
            //lấy bản ghi tương ứng với id truyền vào
            News record = db.News.FirstOrDefault(item => item.Id == id);
            return View("Detail", record);
        }
    }
}
