using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop4DVDS.Models;
using OnlineShop4DVDS.SqlDbContext;

namespace OnlineShop4DVDS.Controllers
{
    public class AdminController : Controller
    {
        SqlContext sqlContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(SqlContext sqlContext, ILogger<AdminController> logger)
        {
            this.sqlContext = sqlContext;
            this._logger = logger;
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
            var artists = sqlContext.Artists.Include(a => a.ArtistRole).ToList();
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

            return RedirectToAction("ArtistView");
        }

        //Artist Update

        public IActionResult ArtistUpdate(int id)
        {
            var artist = sqlContext.Artists.Include(a => a.ArtistRole).FirstOrDefault(c => c.ArtistId == id);
            if(artist == null)
            {
                return View("ArtistView");
            }

            ViewBag.ArtistRoles = new SelectList(sqlContext.ArtistRoles.ToList(), "ArtistRoleId", "ArtistRoleName", artist.ArtistRoleId);

            return View(artist);
        }

        [HttpPost]
        public IActionResult ArtistUpdate(Artist artist)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ArtistRoles = new SelectList(sqlContext.ArtistRoles.ToList(), "ArtistRoleId", "ArtistRoleName", artist.ArtistRoleId);
                return View(artist);
            }

            var existingArtist = sqlContext.Artists.FirstOrDefault(a => a.ArtistId == artist.ArtistId);
            if(existingArtist == null)
            {
                return NotFound();
            }

            existingArtist.ArtistName = artist.ArtistName;
            existingArtist.ArtistAge = artist.ArtistAge;
            existingArtist.ArtistDateOfBirth = artist.ArtistDateOfBirth;
            existingArtist.ArtistBio = artist.ArtistBio;
            existingArtist.ArtistImage = artist.ArtistImage;
            existingArtist.ArtistRoleId = artist.ArtistRoleId;

            sqlContext.Artists.Update(existingArtist);
            sqlContext.SaveChanges();

            return RedirectToAction("ArtistView");
        }

        //Artist Delete

        [HttpPost]
        public IActionResult ArtistDelete(int id)
        {
            var artist = sqlContext.Artists.FirstOrDefault(a => a.ArtistId == id);
            if(artist == null)
            {
                return NotFound();
            }

            sqlContext.Artists.Remove(artist);
            sqlContext.SaveChanges();

            return RedirectToAction("ArtistView");
        }

        //Album View

        public IActionResult AlbumView()
        {
            var albums = sqlContext.Albums.Include(a => a.Artist).ToList();
            return View(albums);
        }

        //Album Insert

        public IActionResult AlbumInsert()
        {
            ViewBag.Artists = sqlContext.Artists.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AlbumInsert(Album album)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Artists = sqlContext.Artists.ToList();
                return View("AlbumInsert");
            }

            sqlContext.Albums.Add(album);
            sqlContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
