using Humanizer.Localisation;
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
                return View("CategoryInsert");
            }

            if (sqlContext.Categories.Any(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            {
                ModelState.AddModelError("CategoryName", "Category already exists");
                return View("Category");
            }

            sqlContext.Categories.Add(category);
            sqlContext.SaveChanges();
            ModelState.Clear();

            return RedirectToAction("CategoryView");
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
                return View(category);
            }

            var existingCategory = sqlContext.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existingCategory == null)
            {
                return RedirectToAction("CategoryView");
            }

            existingCategory.CategoryName = category.CategoryName;
            sqlContext.Categories.Update(existingCategory);
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

            if (sqlContext.Artists.Any(a => a.ArtistName.ToLower() == artist.ArtistName.ToLower()))
            {
                ModelState.AddModelError("ArtistName", "Artist already exists");
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

            if (sqlContext.Albums.Any(a => a.AlbumTitle.ToLower() == album.AlbumTitle.ToLower()))
            {
                ModelState.AddModelError("AlbumTitle", "Album already exists");
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

            if (sqlContext.Songs.Any(s => s.SongName.ToLower() == song.SongName.ToLower()))
            {
                ModelState.AddModelError("SongName", "Song already exists");
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

        //Developer View

        public IActionResult DeveloperView()
        {
            var developers = sqlContext.Developers.ToList();
            return View(developers);
        }

        //Developer Insert

        public IActionResult DeveloperInsert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeveloperInsert(Developer developer)
        {
            if (!ModelState.IsValid)
            {
                return View("DeveloperInsert");
            }

            if (sqlContext.Developers.Any(d => d.DeveloperName.ToLower() == developer.DeveloperName.ToLower()))
            {
                ModelState.AddModelError("DeveloperName", "Developer already exists");
                return View("DeveloperInsert");
            }

            sqlContext.Developers.Add(developer);
            sqlContext.SaveChanges();
            return RedirectToAction("DeveloperView");
        }

        //Developer Update

        public IActionResult DeveloperUpdate(int id)
        {
            var developer = sqlContext.Developers.FirstOrDefault(d => d.DeveloperId == id);
            return View(developer);
        }

        [HttpPost]
        public IActionResult DeveloperUpdate(Developer developer)
        {
            if (!ModelState.IsValid)
            {
                return View(developer);
            }

            var existingDeveloper = sqlContext.Developers.FirstOrDefault(d => d.DeveloperId == developer.DeveloperId);

            existingDeveloper.DeveloperName = developer.DeveloperName;

            sqlContext.Developers.Update(existingDeveloper);
            sqlContext.SaveChanges();

            return RedirectToAction("DeveloperView");
        }

        //Developer Delete

        [HttpPost]
        public IActionResult DeveloperDelete(int id)
        {
            var developer = sqlContext.Developers.FirstOrDefault(d => d.DeveloperId == id);
            if (developer == null)
            {
                return RedirectToAction("DeveloperView");
            }

            sqlContext.Developers.Remove(developer);
            sqlContext.SaveChanges();

            return RedirectToAction("DeveloperView");
        }

        //Genre View

        public IActionResult GenreView()
        {
            var genres = sqlContext.Genres.ToList();
            return View(genres);
        }

        //Genre Insert

        public IActionResult GenreInsert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenreInsert(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View("GenreInsert");
            }

            if (sqlContext.Genres.Any(g => g.GenreName.ToLower() == genre.GenreName.ToLower()))
            {
                ModelState.AddModelError("GenreName", "Genre already exists");
                return View("GenreInsert");
            }

            sqlContext.Genres.Add(genre);
            sqlContext.SaveChanges();
            return RedirectToAction("GenreView");
        }

        //Genre Update

        public IActionResult GenreUpdate(int id)
        {
            var genre = sqlContext.Genres.FirstOrDefault(g => g.GenreId == id);
            return View(genre);
        }

        [HttpPost]
        public IActionResult GenreUpdate(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }

            var existingGenre = sqlContext.Genres.FirstOrDefault(e => e.GenreId == genre.GenreId);

            existingGenre.GenreName = genre.GenreName;

            sqlContext.Genres.Update(existingGenre);
            sqlContext.SaveChanges();

            return RedirectToAction("GameView");
        }

        //Genre Delete

        public IActionResult GenreDelete(int id)
        {
            var genre = sqlContext.Genres.FirstOrDefault(g => g.GenreId == id);
            if (genre == null)
            {
                return RedirectToAction("GenreView");
            }

            sqlContext.Genres.Remove(genre);
            sqlContext.SaveChanges();
            return RedirectToAction("GenreView");
        }

        //Platform View

        public IActionResult PlatformView()
        {
            var platforms = sqlContext.Platforms.ToList();
            return View(platforms);
        }

        //Platform Insert

        public IActionResult PlatformInsert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PlatformInsert(Platform platform)
        {
            if (!ModelState.IsValid)
            {
                return View("PlatformInsert");
            }

            if (sqlContext.Platforms.Any(p => p.PlatformName.ToLower() == platform.PlatformName.ToLower()))
            {
                ModelState.AddModelError("PlatformName", "Platform already exists");
                return View(platform);
            }

            sqlContext.Platforms.Add(platform);
            sqlContext.SaveChanges();
            return RedirectToAction("PlatformView");
        }

        //Platform Update

        public IActionResult PlatformUpdate(int id)
        {
            var platform = sqlContext.Platforms.FirstOrDefault(p => p.PlatformId == id);
            return View(platform);
        }

        [HttpPost]
        public IActionResult PlatformUpdate(Platform platform)
        {
            if (!ModelState.IsValid)
            {
                return View(platform);
            }

            var existingPlatform = sqlContext.Platforms.FirstOrDefault(p => p.PlatformId == platform.PlatformId);

            existingPlatform.PlatformName = platform.PlatformName;

            sqlContext.Platforms.Update(existingPlatform);
            sqlContext.SaveChanges();
            return RedirectToAction("PlatformView");
        }

        //Platform Delete

        [HttpPost]
        public IActionResult PlatformDelete(int id)
        {
            var platform = sqlContext.Platforms.FirstOrDefault(p => p.PlatformId == id);
            if (platform == null)
            {
                return RedirectToAction("PlatformView");
            }
            sqlContext.Platforms.Remove(platform);
            sqlContext.SaveChanges();
            return RedirectToAction("PlatformView");
        }

        //Game View

        public IActionResult GameView()
        {
            var games = sqlContext.Games
                .Include(d => d.Developer)
                .Include(gp => gp.GamePlatforms)
                    .ThenInclude(p => p.Platform)
                .Include(gg => gg.GameGenres)
                    .ThenInclude(g => g.Genre)
                .ToList();

            return View(games);
        }

        //Game Insert

        public IActionResult GameInsert()
        {
            ViewBag.Developers = sqlContext.Developers.ToList();
            ViewBag.Genres = sqlContext.Genres.ToList();
            ViewBag.Platforms = sqlContext.Platforms.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult GameInsert(Game game, List<int> SelectedGenreIds, List<int> SelectedPlatformIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Developers = sqlContext.Developers.ToList();
                ViewBag.Genres = sqlContext.Genres.ToList();
                ViewBag.Platforms = sqlContext.Platforms.ToList();
                return View(game);
            }

            if (sqlContext.Games.Any(g => g.GameName.ToLower() == game.GameName.ToLower()))
            {
                ModelState.AddModelError("GameName", "Game already exists");
                return View(game);
            }

            foreach (var genreId in SelectedGenreIds)
            {
                var genre = sqlContext.Genres.Find(genreId);
                if (genre != null)
                {
                    game.GameGenres.Add(new GameGenre { Genre = genre });
                }
            }

            foreach (var platformId in SelectedPlatformIds)
            {
                var platform = sqlContext.Platforms.Find(platformId);
                if (platform != null)
                {
                    game.GamePlatforms.Add(new GamePlatform { Platform = platform });
                }
            }

            sqlContext.Games.Add(game);
            sqlContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
