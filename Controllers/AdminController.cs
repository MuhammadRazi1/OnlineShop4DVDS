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

        //Category View

        public IActionResult CategoryView()
        {
            var categories = sqlContext.Categories.ToList();
            return View(categories);
        }

        //Category Insert

        public IActionResult CategoryInsert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CategoryInsert(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View("Category");
            }

            if (sqlContext.Categories.Any(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            {
                ModelState.AddModelError("CategoryName", "Category already exists");
                return View("Category");
            }

            sqlContext.Categories.Add(category);
            sqlContext.SaveChanges();
            ModelState.Clear();

            return View("Category");
        }

        //Category Update

        public IActionResult CategoryUpdate(int id)
        {
            var category = sqlContext.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return RedirectToAction("CategoryView");
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult CategoryUpdate(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View("CategoryView");
            }

            var existingCategory = sqlContext.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existingCategory == null)
            {
                return RedirectToAction("CategoryView");
            }

            existingCategory.CategoryName = category.CategoryName;
            sqlContext.SaveChanges();

            return RedirectToAction("CategoryView");
        }

        //Category Delete

        [HttpPost]
        public IActionResult CategoryDelete(int id)
        {
            var category = sqlContext.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return RedirectToAction("CategoryView");
            };

            sqlContext.Categories.Remove(category);
            sqlContext.SaveChanges();

            return RedirectToAction("CategoryView");
        }

        //Artist View

        public IActionResult ArtistView()
        {
            var artists = sqlContext.Artists.ToList();
            return View(artists);
        }

        //Artist Insert

        public IActionResult ArtistInsert()
        {
            ViewBag.ArtistRoles = sqlContext.ArtistRoles.ToList();
            return View();
        }

        [HttpPost]

        public IActionResult ArtistInsert(Artist artist)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ArtistRoles = sqlContext.ArtistRoles.ToList();
                return View("ArtistInsert");
            }

            sqlContext.Artists.Add(artist);
            sqlContext.SaveChanges();
            ModelState.Clear();

            return RedirectToAction("ArtistView");
        }
    }
}
