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
        IWebHostEnvironment env;

        public AdminController(SqlContext sqlContext, ILogger<AdminController> logger, IWebHostEnvironment env)
        {
            this.sqlContext = sqlContext;
            this._logger = logger;
            this.env = env;
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
        public IActionResult AlbumInsert(Album album, IFormFile AlbumImage)
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

            string fileName = "";
            string destination = Path.Combine(env.WebRootPath, "images/albums");
            fileName = Guid.NewGuid().ToString()+"_"+AlbumImage.FileName;
            string filePath = Path.Combine(destination, fileName);
            AlbumImage.CopyTo(new FileStream(filePath, FileMode.Create));

            album.AlbumImage = fileName;
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
        public IActionResult AlbumUpdate(Album album, IFormFile AlbumImage)
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

            if (AlbumImage != null && AlbumImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingAlbum.AlbumImage))
                {
                    string oldImagePath = Path.Combine(env.WebRootPath, "images/albums", existingAlbum.AlbumImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string fileName = Guid.NewGuid().ToString() + "_" + AlbumImage.FileName;
                string destination = Path.Combine(env.WebRootPath, "images/albums");
                string filePath = Path.Combine(destination, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    AlbumImage.CopyTo(stream);
                }

                existingAlbum.AlbumImage = fileName;
            }

            sqlContext.Albums.Update(existingAlbum);
            sqlContext.SaveChanges();

            return RedirectToAction("AlbumView");
        }

        //Album Delete

        [HttpPost]
        public IActionResult AlbumDelete(int id)
        {
            var album = sqlContext.Albums.Include(a => a.Reviews).FirstOrDefault(a => a.AlbumId == id);
            if (album == null)
            {
                return View("AlbumView");
            }

            sqlContext.Reviews.RemoveRange(album.Reviews);
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
        public IActionResult SongInsert(Song song, IFormFile SongFilePath)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                ViewBag.Categories = sqlContext.Categories.ToList();
                ViewBag.Albums = sqlContext.Albums.ToList();
                return View(song);
            }

            if (sqlContext.Songs.Any(s => s.SongName.ToLower() == song.SongName.ToLower()))
            {
                ModelState.AddModelError("SongName", "Song already exists");
                return View(song);
            }

            string videoFileName = Guid.NewGuid().ToString() + "_" + SongFilePath.FileName;
            string videoDestination = Path.Combine(env.WebRootPath, "videos/songs");
            string videoFilePath = Path.Combine(videoDestination, videoFileName);
            SongFilePath.CopyTo(new FileStream(videoFilePath, FileMode.Create));
            song.SongFilePath = videoFileName;

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
        public IActionResult SongUpdate(Song song, IFormFile SongFilePath)
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

            string videoFileName = Guid.NewGuid().ToString() + "_" + SongFilePath.FileName;
            string videoDestination = Path.Combine(env.WebRootPath, "videos/songs");
            string videoFilePath = Path.Combine(videoDestination, videoFileName);

            using (var stream = new FileStream(videoFilePath, FileMode.Create))
            {
                SongFilePath.CopyTo(stream);
            }

            existingSong.SongFilePath = videoFileName;

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

            return RedirectToAction("GenreView");
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
        public IActionResult GameInsert(Game game, List<int> SelectedGenreIds, List<int> SelectedPlatformIds, IFormFile GameImage, IFormFile GameFilePath)
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

            string fileName = "";
            string destination = Path.Combine(env.WebRootPath, "images/games");
            fileName = Guid.NewGuid().ToString() + "_" + GameImage.FileName;
            string filePath = Path.Combine(destination, fileName);
            GameImage.CopyTo(new FileStream(filePath, FileMode.Create));
            game.GameImage = fileName;

            string videoFileName = Guid.NewGuid().ToString() + "_" + GameFilePath.FileName;
            string videoDestination = Path.Combine(env.WebRootPath, "videos/games");
            string videoFilePath = Path.Combine(videoDestination, videoFileName);
            GameFilePath.CopyTo(new FileStream(videoFilePath, FileMode.Create));
            game.GameFilePath = videoFileName;

            sqlContext.Games.Add(game);
            sqlContext.SaveChanges();

            return RedirectToAction("GameView");
        }

        //Game Update

        public IActionResult GameUpdate(int id)
        {
            var game = sqlContext.Games
                .Include(d => d.Developer)
                .Include(gp => gp.GamePlatforms)
                    .ThenInclude(p => p.Platform)
                .Include(gg => gg.GameGenres)
                    .ThenInclude(g => g.Genre)
                .FirstOrDefault(g => g.GameId == id);

            if(game == null)
            {
                return RedirectToAction("GameView");
            }

            ViewBag.Developers = new SelectList(sqlContext.Developers.ToList(), "DeveloperId", "DeveloperName", game.DeveloperId);
            ViewBag.Genres = sqlContext.Genres.ToList();
            ViewBag.Platforms = sqlContext.Platforms.ToList();

            ViewBag.SelectedGenreIds = game.GameGenres.Select(g => g.GenreId).ToList();
            ViewBag.SelectedPlatformIds = game.GamePlatforms.Select(p => p.PlatformId).ToList();

            return View(game);
        }

        [HttpPost]
        public IActionResult GameUpdate(Game game, List<int> SelectedGenreIds, List<int> SelectedPlatformIds, IFormFile GameImage, IFormFile GameFilePath)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }   

                ViewBag.Developers = new SelectList(sqlContext.Developers.ToList(), "DeveloperId", "DeveloperName", game.DeveloperId);
                ViewBag.Genres = sqlContext.Genres.ToList();
                ViewBag.Platforms = sqlContext.Platforms.ToList();
                ViewBag.SelectedGenreIds = SelectedGenreIds;
                ViewBag.SelectedPlatformIds = SelectedPlatformIds;
                return View(game);
            }

            var existingGame = sqlContext.Games
                .Include(gg => gg.GameGenres)
                .Include(gp => gp.GamePlatforms)
                .FirstOrDefault(g => g.GameId == game.GameId);

            if(existingGame == null)
            {
                return View(game);
            }

            existingGame.DeveloperId = game.DeveloperId;
            existingGame.GameName = game.GameName;
            existingGame.GameDescription = game.GameDescription;
            existingGame.GamePrice = game.GamePrice;
            existingGame.GameReleaseDate = game.GameReleaseDate;
            existingGame.GameImage = game.GameImage;
            existingGame.GameFilePath = game.GameFilePath;

            existingGame.GameGenres.Clear();
            foreach(var genreId in SelectedGenreIds)
            {
                var genre = sqlContext.Genres.Find(genreId);
                if(genre != null)
                {
                    existingGame.GameGenres.Add(new GameGenre { Genre = genre });
                }
            }

            existingGame.GamePlatforms.Clear();
            foreach(var platformId in SelectedPlatformIds)
            {
                var platform = sqlContext.Platforms.Find(platformId);
                if(platform != null)
                {
                    existingGame.GamePlatforms.Add(new GamePlatform { Platform = platform });
                }
            }

            if (GameImage != null && GameImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingGame.GameImage))
                {
                    string oldImagePath = Path.Combine(env.WebRootPath, "images/games", existingGame.GameImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string fileName = Guid.NewGuid().ToString() + "_" + GameImage.FileName;
                string destination = Path.Combine(env.WebRootPath, "images/games");
                string filePath = Path.Combine(destination, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    GameImage.CopyTo(stream);
                }

                existingGame.GameImage = fileName;
            }

            if (GameFilePath != null && GameFilePath.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingGame.GameFilePath))
                {
                    string oldVideoPath = Path.Combine(env.WebRootPath, "videos/games", existingGame.GameFilePath);
                    if (System.IO.File.Exists(oldVideoPath))
                    {
                        System.IO.File.Delete(oldVideoPath);
                    }
                }

                string videoFileName = Guid.NewGuid().ToString() + "_" + GameFilePath.FileName;
                string videoDestination = Path.Combine(env.WebRootPath, "videos/games");
                string videoFilePath = Path.Combine(videoDestination, videoFileName);

                using (var stream = new FileStream(videoFilePath, FileMode.Create))
                {
                    GameFilePath.CopyTo(stream);
                }

                existingGame.GameFilePath = videoFileName;
            }

            sqlContext.SaveChanges();

            return RedirectToAction("GameView");
        }

        //Game Delete

        [HttpPost]
        public IActionResult GameDelete(int id)
        {
            var game = sqlContext.Games
                .Include(gg => gg.GameGenres)
                .Include(gp => gp.GamePlatforms)
                .Include(g => g.Reviews)
                .FirstOrDefault(g => g.GameId == id);

            if(game == null)
            {
                return NotFound();
            }

            sqlContext.Reviews.RemoveRange(game.Reviews);
            sqlContext.GameGenres.RemoveRange(game.GameGenres);
            sqlContext.GamePlatforms.RemoveRange(game.GamePlatforms);
            sqlContext.Games.Remove(game);
            sqlContext.SaveChanges();

            return RedirectToAction("GameView");
        }

        //Movie View

        public IActionResult MovieView()
        {
            var movies = sqlContext.Movies
                 .Include(mg => mg.MovieGenres)
                     .ThenInclude(g => g.Genre)
                 .ToList();

            return View(movies);
        }

        ////Movie Insert
        
        public IActionResult MovieInsert()
        {
            ViewBag.Genres = sqlContext.Genres.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult MovieInsert(Movie movie, List<int> SelectedGenreIds, IFormFile MovieImage, IFormFile MovieFilePath)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = sqlContext.Genres.ToList();
                return View(movie);
            }

            foreach (var genreId in SelectedGenreIds)
            {
                var genre = sqlContext.Genres.Find(genreId);
                if (genre != null)
                {
                    movie.MovieGenres.Add(new MovieGenre { Genre = genre });
                }
            }

            string fileName = "";
            string destination = Path.Combine(env.WebRootPath, "images/movies");
            fileName = Guid.NewGuid().ToString() + "_" + MovieImage.FileName;
            string filePath = Path.Combine(destination, fileName);
            MovieImage.CopyTo(new FileStream(filePath, FileMode.Create));
            movie.MovieImage = fileName;

            string videoFileName = Guid.NewGuid().ToString() + "_" + MovieFilePath.FileName;
            string videoDestination = Path.Combine(env.WebRootPath, "videos/movies");
            string videoFilePath = Path.Combine(videoDestination, videoFileName);
            MovieFilePath.CopyTo(new FileStream(videoFilePath, FileMode.Create));
            movie.MovieFilePath = videoFileName;

            sqlContext.Movies.Add(movie);
            sqlContext.SaveChanges();
            return RedirectToAction("MovieView");
        }

        //Movie Update

        public IActionResult MovieUpdate(int id)
        {
            var movie = sqlContext.Movies
                 .Include(mg => mg.MovieGenres)
                     .ThenInclude(g => g.Genre)
                 .FirstOrDefault(m => m.MovieId == id);

            if (movie == null)
            {
                return RedirectToAction("MovieView");
            }

            ViewBag.Genres = sqlContext.Genres.ToList();

            ViewBag.SelectedGenreIds = movie.MovieGenres.Select(g => g.GenreId).ToList();

            return View(movie);
        }

        [HttpPost]
        public IActionResult MovieUpdate(Movie movie, List<int> SelectedGenreIds, IFormFile MovieImage, IFormFile MovieFilePath)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = sqlContext.Genres.ToList();
                ViewBag.SelectedGenreIds = SelectedGenreIds;
                return View(movie);
            }

            var existingMovie = sqlContext.Movies
                .Include(gg => gg.MovieGenres)
                .FirstOrDefault(g => g.MovieId == movie.MovieId);

            if (existingMovie == null)
            {
                return View(movie);
            }

            existingMovie.MovieTitle = movie.MovieTitle;
            existingMovie.MovieDescription = movie.MovieDescription;
            existingMovie.MovieReleaseDate = movie.MovieReleaseDate;
            existingMovie.MoviePrice = movie.MoviePrice;
            existingMovie.MovieFilePath = movie.MovieFilePath;
            existingMovie.MovieImage = movie.MovieImage;
            existingMovie.MovieRating = movie.MovieRating;

            existingMovie.MovieGenres.Clear();
            foreach (var genreId in SelectedGenreIds)
            {
                var genre = sqlContext.Genres.Find(genreId);
                if (genre != null)
                {
                    existingMovie.MovieGenres.Add(new MovieGenre { Genre = genre });
                }
            }

            if (MovieImage != null && MovieImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingMovie.MovieImage))
                {
                    string oldImagePath = Path.Combine(env.WebRootPath, "images/movies", existingMovie.MovieImage);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string fileName = Guid.NewGuid().ToString() + "_" + MovieImage.FileName;
                string destination = Path.Combine(env.WebRootPath, "images/movies");
                string filePath = Path.Combine(destination, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    MovieImage.CopyTo(stream);
                }

                existingMovie.MovieImage = fileName;
            }

            if (MovieFilePath != null && MovieFilePath.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingMovie.MovieFilePath))
                {
                    string oldVideoPath = Path.Combine(env.WebRootPath, "videos/movies", existingMovie.MovieFilePath);
                    if (System.IO.File.Exists(oldVideoPath))
                    {
                        System.IO.File.Delete(oldVideoPath);
                    }
                }

                string videoFileName = Guid.NewGuid().ToString() + "_" + MovieFilePath.FileName;
                string videoDestination = Path.Combine(env.WebRootPath, "videos/movies");
                string videoFilePath = Path.Combine(videoDestination, videoFileName);

                using (var stream = new FileStream(videoFilePath, FileMode.Create))
                {
                    MovieFilePath.CopyTo(stream);
                }

                existingMovie.MovieFilePath = videoFileName;
            }

            sqlContext.SaveChanges();
            return RedirectToAction("MovieView");
        }

        //Movie Delete

        public IActionResult MovieDelete(int id)
        {
            var movie = sqlContext.Movies
                .Include(gg => gg.MovieGenres)
                .Include(m => m.Reviews)
                .FirstOrDefault(g => g.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            sqlContext.Reviews.RemoveRange(movie.Reviews);
            sqlContext.MovieGenres.RemoveRange(movie.MovieGenres);
            sqlContext.Movies.Remove(movie);
            sqlContext.SaveChanges();

            return RedirectToAction("MovieView");
        }

        //News View

        public IActionResult NewsView()
        {
            var news = sqlContext.News.ToList();
            return View(news);
        }

        //News Insert

        public IActionResult NewsInsert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewsInsert(News news)
        {
            if (!ModelState.IsValid)
            {
                return View(news);
            }

            sqlContext.News.Add(news);
            sqlContext.SaveChanges();

            return RedirectToAction("NewsView");
        }

        //News Update

        public IActionResult NewsUpdate(int id)
        {
            var news = sqlContext.News.FirstOrDefault(n => n.NewsId == id);

            if(news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        [HttpPost]
        public IActionResult NewsUpdate(News news)
        {
            if(!ModelState.IsValid)
            {
                return View(news);
            }

            var existingNews = sqlContext.News.FirstOrDefault(n => n.NewsId == news.NewsId);
            if(existingNews == null)
            {
                return NotFound();
            }

            existingNews.NewsTitle = news.NewsTitle;
            existingNews.NewsDescription = news.NewsDescription;
            existingNews.NewsDate = news.NewsDate;
            existingNews.NewsAuthor = news.NewsAuthor;

            sqlContext.News.Update(existingNews);
            sqlContext.SaveChanges();
            return RedirectToAction("NewsView");
        }

        //News Delete

        [HttpPost]
        public IActionResult NewsDelete(int id)
        {
            var news = sqlContext.News.FirstOrDefault(n => n.NewsId == id);
            if(news == null)
            {
                return NotFound();
            }

            sqlContext.News.Remove(news);
            sqlContext.SaveChanges();
            return RedirectToAction("NewsView");
        }

        //Feedback View

        public IActionResult FeedbackView()
        {
            var feedbacks = sqlContext.Feedbacks.Include(u => u.User).ToList();
            return View(feedbacks);
        }

        //Feedback delete

        [HttpPost]
        public IActionResult FeedbackDelete(int id)
        {
            var feedback = sqlContext.Feedbacks.FirstOrDefault(f => f.FeedbackId == id);
            if(feedback == null)
            {
                return NotFound();
            }

            sqlContext.Feedbacks.Remove(feedback);
            sqlContext.SaveChanges();
            return RedirectToAction("FeedbackView");
        }

        //User View

        public IActionResult UserView()
        {
            var users = sqlContext.Users.ToList();
            return View(users);
        }

        public IActionResult UserUpdate(int id)
        {
            var user = sqlContext.Users.FirstOrDefault(u => u.UserId == id);
            return View(user);
        }

        [HttpPost]
        public IActionResult UserUpdate(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = sqlContext.Users.FirstOrDefault(c => c.UserId == user.UserId);
            if (existingUser == null)
            {
                return RedirectToAction("UserView");
            }

            existingUser.UserName = user.UserName;
            existingUser.UserEmail = user.UserEmail;
            existingUser.UserRole = user.UserRole;
            sqlContext.Users.Update(existingUser);
            sqlContext.SaveChanges();

            return RedirectToAction("UserView");
        }

        //Supplier

        public IActionResult SupplierView()
        {
            var supplier = sqlContext.Suppliers.ToList();
            return View(supplier);
        }

        public IActionResult SupplierInsert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SupplierInsert(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            sqlContext.Suppliers.Add(supplier);
            sqlContext.SaveChanges();
            return RedirectToAction("SupplierView");
        }

        public IActionResult SupplierUpdate(int id)
        {
            var supplier = sqlContext.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            return View(supplier);
        }

        [HttpPost]
        public IActionResult SupplierUpdate(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            var existingSupplier = sqlContext.Suppliers.FirstOrDefault(s => s.SupplierId == supplier.SupplierId);
            if(existingSupplier == null)
            {
                return View(supplier);
            }

            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.SupplierEmail = supplier.SupplierEmail;
            existingSupplier.SupplierPhone = supplier.SupplierPhone;
            sqlContext.Suppliers.Update(existingSupplier);
            sqlContext.SaveChanges();
            return RedirectToAction("SupplierView");
        }

        [HttpPost]
        public IActionResult SupplierDelete(int id)
        {
            var supplier = sqlContext.Suppliers.FirstOrDefault(s => s.SupplierId == id);
            sqlContext.Suppliers.Remove(supplier);
            sqlContext.SaveChanges();
            return RedirectToAction("SupplierView");
        }

        //Producer

        public IActionResult ProducerView()
        {
            var producer = sqlContext.Producers.ToList();
            return View(producer);
        }

        public IActionResult ProducerInsert()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProducerInsert(Producer producer)
        {
            if (!ModelState.IsValid)
            {
                return View(producer);
            }

            sqlContext.Producers.Add(producer);
            sqlContext.SaveChanges();
            return RedirectToAction("ProducerView");
        }

        public IActionResult ProducerUpdate(int id)
        {
            var producer = sqlContext.Producers.FirstOrDefault(s => s.ProducerId == id);
            return View(producer);
        }

        [HttpPost]
        public IActionResult ProducerUpdate(Producer producer)
        {
            if (!ModelState.IsValid)
            {
                return View(producer);
            }

            var existingProducer = sqlContext.Producers.FirstOrDefault(s => s.ProducerId == producer.ProducerId);
            if (existingProducer == null)
            {
                return View(producer);
            }

            existingProducer.ProducerName = producer.ProducerName;
            existingProducer.ProducerEmail = producer.ProducerEmail;
            existingProducer.ProducerPhone = producer.ProducerPhone;
            sqlContext.Producers.Update(existingProducer);
            sqlContext.SaveChanges();
            return RedirectToAction("ProducerView");
        }

        [HttpPost]
        public IActionResult ProducerDelete(int id)
        {
            var producer = sqlContext.Producers.FirstOrDefault(s => s.ProducerId == id);
            sqlContext.Producers.Remove(producer);
            sqlContext.SaveChanges();
            return RedirectToAction("ProducerView");
        }

        //Order
        public IActionResult OrderView()
        {
            var orders = sqlContext.Orders
                .Include(o => o.CartItems)
                .Join(sqlContext.Users,
                      order => order.UserId,
                      user => user.UserId,
                      (order, user) => new
                      {
                          order.OrderId,
                          order.OrderDate,
                          order.TotalAmount,
                          order.Phone,
                          order.Country,
                          order.City,
                          order.Address,
                          UserEmail = user.UserEmail,
                          Fulfilled = order.Fulfilled // Include Fulfilled Status
                      })
                .ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult ToggleOrderStatus(int id)
        {
            var order = sqlContext.Orders.Find(id);
            if (order == null)
            {
                return Json(new { success = false });
            }

            order.Fulfilled = !order.Fulfilled;
            sqlContext.SaveChanges();

            return Json(new { success = true, newStatus = order.Fulfilled });
        }



    }
}
