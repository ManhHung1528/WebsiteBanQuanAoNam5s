using Microsoft.AspNetCore.Mvc;
using Website.Areas.Admin.Attributes;


namespace Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AccessDenied() 
        { 
            return View(); 
        }
    }
    }

