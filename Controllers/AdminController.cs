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

            return View("CategoryView");
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
            if (artist == null)
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
            if (existingArtist == null)
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
            if (artist == null)
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

            return RedirectToAction("AlbumView");
        }

        //Album Update

        public IActionResult AlbumUpdate(int id)
        {
            var album = sqlContext.Albums.Include(a => a.Artist).FirstOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                return View("AlbumView");
            }

            ViewBag.Artists = new SelectList(sqlContext.Artists.ToList(), "ArtistId", "ArtistName", album.ArtistId);

            return View(album);
        }

        [HttpPost]
        public IActionResult AlbumUpdate(Album album)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Artists = new SelectList(sqlContext.Artists.ToList(), "ArtistId", "ArtistName", album.ArtistId);
                return View(album);
            }

            var existingAlbum = sqlContext.Albums.FirstOrDefault(a => a.AlbumId == album.AlbumId);
            if (existingAlbum == null)
            {
                return NotFound();
            }

            existingAlbum.AlbumTitle = album.AlbumTitle;
            existingAlbum.AlbumReleaseDate = album.AlbumReleaseDate;
            existingAlbum.AlbumDescription = album.AlbumDescription;
            existingAlbum.AlbumPrice = album.AlbumPrice;
            existingAlbum.AlbumImage = album.AlbumImage;
            existingAlbum.ArtistId = album.ArtistId;

            sqlContext.Albums.Update(existingAlbum);
            sqlContext.SaveChanges();

            return RedirectToAction("AlbumView");
        }

        //Album Delete

        [HttpPost]
        public IActionResult AlbumDelete(int id)
        {
            var album = sqlContext.Albums.FirstOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                return View("AlbumView");
            }

            sqlContext.Albums.Remove(album);
            sqlContext.SaveChanges();

            return RedirectToAction("AlbumView");
        }

        //Song View

        public IActionResult SongView()
        {
            var songs = sqlContext.Songs.Include(c => c.Category).Include(a => a.Album).ToList();
            return View(songs);
        }

        //Song Insert

        public IActionResult SongInsert()
        {
            var categories = sqlContext.Categories.ToList();
            var albums = sqlContext.Albums.ToList();

            ViewBag.Categories = categories;
            ViewBag.Albums = albums;
            return View();
        }

        [HttpPost]
        public IActionResult SongInsert(Song song)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = sqlContext.Categories.ToList();
                ViewBag.Albums = sqlContext.Albums.ToList();
                return View(song);
            }

            sqlContext.Songs.Add(song);
            sqlContext.SaveChanges();

            return RedirectToAction("SongView");
        }

        //Song Update

        public IActionResult SongUpdate(int id)
        {
            var song = sqlContext.Songs.Include(c => c.Category).Include(a => a.Album).FirstOrDefault(s => s.SongId == id);
            if (song == null)
            {
                return View("SongView");
            }

            ViewBag.Categories = new SelectList(sqlContext.Categories.ToList(), "CategoryId", "CategoryName", song.CategoryId);
            ViewBag.Albums = new SelectList(sqlContext.Albums.ToList(), "AlbumId", "AlbumTitle", song.AlbumId);

            return View(song);
        }

        [HttpPost]
        public IActionResult SongUpdate(Song song)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(sqlContext.Categories.ToList(), "CategoryId", "CategoryName", song.CategoryId);
                ViewBag.Albums = new SelectList(sqlContext.Albums.ToList(), "AlbumId", "AlbumTitle", song.AlbumId);
                return View(song);
            }

            var existingSong = sqlContext.Songs.FirstOrDefault(s => s.SongId == song.SongId);
            if (existingSong == null)
            {
                return NotFound();
            }

            existingSong.SongName = song.SongName;
            existingSong.SongFilePath = song.SongFilePath;
            existingSong.CategoryId = song.CategoryId;
            existingSong.AlbumId = song.AlbumId;

            sqlContext.Songs.Update(existingSong);
            sqlContext.SaveChanges();

            return RedirectToAction("SongView");
        }

        //Song Delete

        [HttpPost]
        public IActionResult SongDelete(int id)
        {
            var song = sqlContext.Songs.FirstOrDefault(s => s.SongId == id);
            if (song == null)
            {
                return View("SongView");
            }

            sqlContext.Songs.Remove(song);
            sqlContext.SaveChanges();

            return RedirectToAction("SongView");
        }
    }
}
