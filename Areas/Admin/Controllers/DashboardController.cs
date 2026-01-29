using Microsoft.AspNetCore.Mvc;
using Website.Models;
using X.PagedList;
using System.Linq;
using System.Globalization;
namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public MyDbContext db = new MyDbContext();

        public IActionResult Index()
        {
            // Lấy số liệu thống kê
            ViewBag.TotalProducts = db.Products.Count();
            ViewBag.TotalCategories = db.Categories.Count();
            ViewBag.TotalOrders = db.Order.Count();
            ViewBag.TotalUsers = db.Users.Count();

            // Thống kê sản phẩm theo danh mục (cho Chart)
            var productByCategory = db.Categories
                .Select(c => new
                {
                    c.Name,
                    Count = db.categoriesProducts.Count(cp => cp.CategoryId == c.Id)
                }).ToList();

            ViewBag.ProductByCategoryNames = productByCategory.Select(x => x.Name).ToArray();
            ViewBag.ProductByCategoryCounts = productByCategory.Select(x => x.Count).ToArray();
            DateTime today = DateTime.Now;

            // Lấy thứ 2 đầu tuần;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            DateTime endOfWeek = startOfWeek.AddDays(7);

            double weeklyRevenue = (
                from o in db.Order
                join od in db.OrdersDetails on o.Id equals od.OrderId
                join p in db.Products on od.ProductId equals p.Id
                where o.Status == 1
                      && o.Create >= startOfWeek
                      && o.Create < endOfWeek
                select (double?)((p.Price - p.Discount))
            ).Sum() ?? 0;

            ViewBag.WeeklyRevenue = weeklyRevenue;
            // ===== BIỂU ĐỒ DOANH THU THEO TUẦN =====
            // Lấy doanh thu từng ngày trong tuần
            var weeklyRevenueChart = (
                from o in db.Order
                join od in db.OrdersDetails on o.Id equals od.OrderId
                join p in db.Products on od.ProductId equals p.Id
                where o.Status == 1
                      && o.Create >= startOfWeek
                      && o.Create < endOfWeek
                group new { od, p } by o.Create.Date into g
                select new
                {
                    Day = g.Key,
                    Total = g.Sum(x => (x.p.Price - x.p.Discount) * x.od.Quantity)
                }
            ).ToList();

            // Chuẩn hoá đủ 7 ngày (kể cả ngày không có đơn)
            var labels = new List<string>();
            var data = new List<double>();

            for (int i = 0; i < 7; i++)
            {
                DateTime day = startOfWeek.AddDays(i);
                labels.Add(day.ToString("dd/MM"));

                var revenue = weeklyRevenueChart
                    .Where(x => x.Day == day.Date)
                    .Select(x => x.Total)
                    .FirstOrDefault();

                data.Add(revenue);
            }

            ViewBag.WeeklyRevenueLabels = labels;
            ViewBag.WeeklyRevenueData = data;

            return View();
        }
    }
}
