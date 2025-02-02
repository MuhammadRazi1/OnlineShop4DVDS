using Microsoft.AspNetCore.Mvc;
using OnlineShop4DVDS.Models;
using OnlineShop4DVDS.SqlDbContext;

namespace OnlineShop4DVDS.Controllers
{
    public class AdminController : Controller
    {
        SqlContext sqlContext;

        public AdminController(SqlContext sqlContext)
        {
            this.sqlContext = sqlContext;
        }

        public IActionResult Index()
        {
            return View();
        }

//Category

        public IActionResult Category()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Category(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View("Category");
            }

            if(sqlContext.Categories.Any(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            {
                ModelState.AddModelError("CategoryName", "Category already exists");
                return View("Category");
            }

            sqlContext.Categories.Add(category);
            sqlContext.SaveChanges();
            ModelState.Clear();

            return View("Category");
        }
    }
}
