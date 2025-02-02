using Microsoft.AspNetCore.Mvc;

namespace OnlineShop4DVDS.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
