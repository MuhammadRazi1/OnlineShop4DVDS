using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.EntityFrameworkCore;
using OnlineShop4DVDS.Models;
using OnlineShop4DVDS.SqlDbContext;
using System.Reflection.Metadata.Ecma335;

namespace OnlineShop4DVDS.Controllers
{
    public class UserController : Controller
    {
        SqlContext sqlContext;

        public UserController(SqlContext sqlContext)
        {
            this.sqlContext = sqlContext;
        }

        //User Register

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(user);
            }

            if (sqlContext.Users.Any(u => u.UserEmail == user.UserEmail))
            {
                ModelState.AddModelError("UserEmail", "Email is already in use.");
                return View(user);
            }

            user.UserPassword = HashPassword(user.UserPassword);
            user.UserRole = "Member";

            sqlContext.Users.Add(user);
            sqlContext.SaveChanges();

            ModelState.Clear();

            Console.WriteLine($"User registered: {user.UserEmail}, Session Role: {HttpContext.Session.GetString("UserRole")}");

            return RedirectToAction("Login");
        }


        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return System.Convert.ToBase64String(bytes);
            }
        }

        //User Login

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User loginUser)
        {
            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == loginUser.UserEmail);

            if (user == null)
            {
                ModelState.AddModelError("UserEmail", "User does not exist");
                return View(loginUser);
            }

            if (user.UserPassword != HashPassword(loginUser.UserPassword))
            {
                ModelState.AddModelError("UserPassword", "Invalid password.");
                return View(loginUser);
            }

            HttpContext.Session.SetString("UserRole", user.UserRole);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserEmail", user.UserEmail);
            HttpContext.Session.SetInt32("UserId", user.UserId);

            Console.WriteLine($"User {user.UserEmail} logged in successfully with role {user.UserRole}");

            if (user.UserRole == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index");
        }

        //User Logout

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.SetString("UserRole", "Guest");

            Console.WriteLine($"User role changed to Session Role: {HttpContext.Session.GetString("UserRole")}");

            return RedirectToAction("Login");
        }

        //Index View

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("UserName");

            var popularMovies = sqlContext.Movies
        .OrderByDescending(m => m.MovieRating)
        .Take(3)
        .Include(mg => mg.MovieGenres)
            .ThenInclude(g => g.Genre)
        .ToList();

            var latestMovies = sqlContext.Movies
        .OrderByDescending(m => m.MovieReleaseDate)
        .Take(3)
        .Include(mg => mg.MovieGenres)
            .ThenInclude(g => g.Genre)
        .ToList();

            var recentSongs = sqlContext.Songs
        .OrderByDescending(s => s.CreatedAt)
        .Take(3)
        .Include(s => s.Album)
        .Include(c => c.Category)
        .ToList();

            ViewBag.RecentSongs = recentSongs;

            ViewBag.LatestMovies = latestMovies;

            ViewBag.PopularMovies = popularMovies;

            ViewBag.UserName = username;
            return View();
        }

        //Profile View

        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("UserName");
            var useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserName = username;
            ViewBag.UserEmail = useremail;

            return View();
        }

        //Profile Update

        [HttpGet]
        public IActionResult ProfileUpdate()
        {
            string? useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == useremail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var orders = sqlContext.Orders
                .Where(o => o.UserId == user.UserId)
                .Include(o => o.CartItems)
                .ToList();

            foreach (var order in orders)
            {
                foreach (var cartItem in order.CartItems)
                {
                    switch (cartItem.ItemType)
                    {
                        case "Game":
                            cartItem.Item = sqlContext.Games
                                .Where(g => g.GameId == cartItem.ItemId)
                                .Select(g => new { Name = g.GameName })
                                .FirstOrDefault();
                            break;
                        case "Movie":
                            cartItem.Item = sqlContext.Movies
                                .Where(m => m.MovieId == cartItem.ItemId)
                                .Select(m => new { Name = m.MovieTitle })
                                .FirstOrDefault();
                            break;
                        case "Album":
                            cartItem.Item = sqlContext.Albums
                                .Where(a => a.AlbumId == cartItem.ItemId)
                                .Select(a => new { Name = a.AlbumTitle })
                                .FirstOrDefault();
                            break;
                    }
                }
            }

            ViewBag.Orders = orders;
            return View(user);
        }

        [HttpPost]
        public IActionResult ProfileUpdate(User updateUser)
        {
            string? useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == useremail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Check if the new email is already in use by another user
            if (sqlContext.Users.Any(u => u.UserEmail == updateUser.UserEmail && u.UserEmail != useremail))
            {
                ModelState.AddModelError("UserEmail", "This email is already in use.");
                return View(updateUser);
            }

            // Update the user's profile
            user.UserEmail = updateUser.UserEmail;
            user.UserName = updateUser.UserName;

            sqlContext.SaveChanges();

            // Update the session with the new user details
            HttpContext.Session.SetString("UserEmail", user.UserEmail);
            HttpContext.Session.SetString("UserName", user.UserName);

            Console.WriteLine($"User email updated to: {HttpContext.Session.GetString("UserEmail")}");
            Console.WriteLine($"User name updated to: {HttpContext.Session.GetString("UserName")}");

            // Redirect back to the ProfileUpdate page
            return RedirectToAction("ProfileUpdate");
        }

        //Account Delete

        [HttpPost]
        public IActionResult DeleteAccount()
        {
            string? useremail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(useremail))
            {
                return RedirectToAction("Login");
            }

            var user = sqlContext.Users.FirstOrDefault(u => u.UserEmail == useremail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            sqlContext.Users.Remove(user);
            sqlContext.SaveChanges();

            HttpContext.Session.Clear();

            return RedirectToAction("Register");
        }

        //Album Page

        [HttpGet]
        public IActionResult AlbumPage(int? artistId)
        {
            var albums = sqlContext.Albums
                .Include(a => a.Artist)
                .AsQueryable(); // Allows dynamic filtering

            // Apply filter if an artist is selected
            if (artistId.HasValue && artistId.Value > 0)
            {
                albums = albums.Where(a => a.ArtistId == artistId);
            }

            // Pass artists list for dropdown filtering
            ViewBag.Artists = sqlContext.Artists.ToList();
            ViewBag.SelectedArtist = artistId;

            return View(albums.ToList());
        }


        public IActionResult SingleAlbum(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var album = sqlContext.Albums
                .Include(a => a.Artist)
                .Include(r => r.Reviews)
                    .ThenInclude(u => u.User)
                .FirstOrDefault(a => a.AlbumId == id);

            if (album == null)
            {
                return NotFound();
            }

            var songs = sqlContext.Songs
                .Where(s => s.AlbumId == id)
                .Include(s => s.Category)
                .ToList();

            var userCollection = new List<int>();
            if (userId.HasValue)
            {
                userCollection = sqlContext.UserSongs
                    .Where(us => us.UserId == userId.Value && songs.Select(s => s.SongId).Contains(us.SongId))
                    .Select(us => us.SongId)
                    .ToList();
            }

            ViewBag.Songs = songs;
            ViewBag.UserCollection = userCollection;
            ViewBag.AlbumId = album.AlbumId;
            ViewBag.Reviews = album.Reviews;

            return View(album);
        }

        //Song Page

        [HttpGet]
        public IActionResult SongPage(int? categoryId, int? albumId)
        {
            var songs = sqlContext.Songs
                .Include(s => s.Category)
                .Include(s => s.Album)
                .AsQueryable();  // Allows dynamic filtering

            // Apply filters if provided
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                songs = songs.Where(s => s.CategoryId == categoryId);
            }
            if (albumId.HasValue && albumId.Value > 0)
            {
                songs = songs.Where(s => s.AlbumId == albumId);
            }

            // Pass category and album lists for the dropdown filters
            ViewBag.Categories = sqlContext.Categories.ToList();
            ViewBag.Albums = sqlContext.Albums.ToList();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedAlbum = albumId;

            return View(songs.ToList());
        }


        public IActionResult SingleSong(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var song = sqlContext.Songs
                .Include(c => c.Category)
                .Include(a => a.Album)
                .FirstOrDefault(s => s.SongId == id);

            if (song == null)
            {
                return NotFound();
            }

            var isInCollection = sqlContext.UserSongs
                .Any(us => us.UserId == userId && us.SongId == id);

            ViewBag.IsInCollection = isInCollection; 

            return View(song);
        }

        public IActionResult SongVideo(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Member")
            {
                return RedirectToAction("Login");
            }
            var song = sqlContext.Songs.FirstOrDefault(s => s.SongId == id);
            return View(song);
        }

        //Game Page

        public IActionResult GamePage()
        {
            var game = sqlContext.Games.ToList();
            return View(game);
        }

        public IActionResult SingleGame(int id)
        {
            var games = sqlContext.Games
                .Include(d => d.Developer)
                .Include(gp => gp.GamePlatforms)
                    .ThenInclude(p => p.Platform)
                .Include(gg => gg.GameGenres)
                    .ThenInclude(g => g.Genre)
                .Include(r => r.Reviews)
                    .ThenInclude(u => u.User)
                .FirstOrDefault(g => g.GameId == id);

            ViewBag.GameId = games.GameId;
            ViewBag.Reviews = games.Reviews;

            return View(games);
        }

        public IActionResult GameTrailer(int id)
        {
            var game = sqlContext.Games
                .FirstOrDefault(g => g.GameId == id);
            return View(game);
        }

        //Movie Page

        [HttpGet]
        public IActionResult MoviePage(int? genreId, string sortBy)
        {
            var movies = sqlContext.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .AsQueryable();

            // Filter by Genre
            if (genreId.HasValue)
            {
                movies = movies.Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId));
            }

            // Sorting Logic
            switch (sortBy)
            {
                case "popular":
                    movies = movies.OrderByDescending(m => m.MovieRating);
                    break;
                case "latest":
                    movies = movies.OrderByDescending(m => m.MovieReleaseDate);
                    break;
                default:
                    movies = movies.OrderBy(m => m.MovieTitle);
                    break;
            }

            ViewBag.Genres = sqlContext.Genres.ToList(); // Pass genres to View
            return View(movies.ToList());
        }


        public IActionResult SingleMovie(int id)
        {
            var movie = sqlContext.Movies
                .Include(mg => mg.MovieGenres)
                    .ThenInclude(g => g.Genre)
                 .Include(r => r.Reviews)
                    .ThenInclude(u => u.User)
                .FirstOrDefault(m => m.MovieId == id);

            ViewBag.MovieId = movie.MovieId;
            ViewBag.Reviews = movie.Reviews;

            return View(movie);
        }

        public IActionResult MovieTrailer(int id)
        {
            var movie = sqlContext.Movies
                .FirstOrDefault(m => m.MovieId == id);
            return View(movie);
        }

        //News Page

        [HttpGet]
        public IActionResult NewsPage(string sortOrder)
        {
            var news = sqlContext.News.AsQueryable();

            // Apply sorting based on the parameter
            switch (sortOrder)
            {
                case "oldest":
                    news = news.OrderBy(n => n.NewsDate);
                    break;
                default: // "latest" is the default
                    news = news.OrderByDescending(n => n.NewsDate);
                    break;
            }

            return View(news.ToList());
        }

        public IActionResult SingleNews(int id)
        {
            var news = sqlContext.News.FirstOrDefault(n => n.NewsId == id);
            return View(news);
        }

        //Feedback Page

        public IActionResult FeedbackPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FeedbackInsert(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return View(feedback);
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            feedback.UserId = userId.Value;
            sqlContext.Feedbacks.Add(feedback);
            sqlContext.SaveChanges();
            ModelState.Clear();
            ViewBag.FeedbackSuccess = "Feedback added successfully!";
            return RedirectToAction("FeedbackPage");
        }

        //Collection Page

        [HttpPost]
        public IActionResult AddToCollection(int id, int albumId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var existingCollection = sqlContext.UserSongs.FirstOrDefault(us => us.UserId == userId && us.SongId == id);

            if(existingCollection == null)
            {
                var userSong = new UserSong
                {
                    UserId = userId.Value,
                    SongId = id,
                    AddedOn = System.DateTime.Now
                };

                sqlContext.UserSongs.Add(userSong);
                sqlContext.SaveChanges();

                TempData["CollectionMessage"] = "Song added to collection!";
                TempData["AddedSongId"] = id;
            }
            else
            {
                TempData["CollectionMessage"] = "Song already in collection!";
            }

            return RedirectToAction("SingleSong", new { id });
        }

        [HttpPost]
        public IActionResult RemoveFromCollection(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var userSong = sqlContext.UserSongs
                .FirstOrDefault(us => us.UserId == userId && us.SongId == id);

            if (userSong != null)
            {
                sqlContext.UserSongs.Remove(userSong);
                sqlContext.SaveChanges();

                TempData["CollectionMessage"] = "Song removed from collection!";
            }
            else
            {
                TempData["CollectionMessage"] = "Song is not in your collection!";
            }

            return RedirectToAction("SingleSong", new { id });
        }

        public IActionResult CollectionView()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var collections = sqlContext.UserSongs
                .Include(s => s.Song)
                .ThenInclude(a => a.Album)
                .Include(c => c.Song.Category)
                .Where(us => us.UserId == userId)
                .ToList();

            if (TempData["CollectionMessage"] != null)
            {
                ViewBag.CollectionMessage = TempData["CollectionMessage"];
            }

            return View(collections);
        }

        //Review

        [HttpPost]
        public IActionResult AddReview(Review review)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                review.ReviewDate = DateTime.UtcNow;
                sqlContext.Reviews.Add(review);
                sqlContext.SaveChanges();

                TempData["ReviewMessage"] = "Review added successfully!";
            }
            else
            {
                TempData["ReviewMessage"] = "Failed to add review. Please check your input.";
            }

            if (review.AlbumId != null)
            {
                return RedirectToAction("SingleAlbum", "User", new { id = review.AlbumId });
            }
            else if (review.GameId != null)
            {
                return RedirectToAction("SingleGame", "User", new { id = review.GameId });
            }
            else if (review.MovieId != null)
            {
                return RedirectToAction("SingleMovie", "User", new { id = review.MovieId });
            }

            return RedirectToAction("Index");
        }

        //Cart

        public IActionResult CartView()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            // Retrieve the user's cart with items
            var cart = sqlContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            // If the cart is null, create an empty list of cart items
            var cartItems = cart?.CartItems ?? new List<CartItem>();

            // Fetch item details (name and image) for each cart item
            var cartItemDetails = new List<CartItemDetailViewModel>();
            foreach (var item in cartItems)
            {
                var itemName = GetItemName(item.ItemType, item.ItemId);
                var itemImage = GetItemImage(item.ItemType, item.ItemId);
                var itemPlatform = GetPlatformName(item.ItemType, item.PlatformId);

                // Debugging: Print item details to the console
                Console.WriteLine($"ItemType: {item.ItemType}, ItemId: {item.ItemId}, ItemName: {itemName}, ItemImage: {itemImage}");

                var itemDetail = new CartItemDetailViewModel
                {
                    CartItem = item,
                    ItemName = itemName,
                    ItemImage = itemImage,
                    PlatformName = itemPlatform
                };
                cartItemDetails.Add(itemDetail);
            }
            var userAddress = sqlContext.UserAddresses.FirstOrDefault(ua => ua.UserId == userId);
            ViewBag.UserAddress = userAddress;
            // Pass the cart item details to the view
            return View(cartItemDetails);
        }

        // Helper method to get item name
        private string GetItemName(string itemType, int itemId)
        {
            switch (itemType)
            {
                case "Game":
                    return sqlContext.Games.FirstOrDefault(g => g.GameId == itemId)?.GameName;
                case "Movie":
                    return sqlContext.Movies.FirstOrDefault(m => m.MovieId == itemId)?.MovieTitle;
                case "Album":
                    return sqlContext.Albums.FirstOrDefault(a => a.AlbumId == itemId)?.AlbumTitle;
                default:
                    return "Unknown Item";
            }
        }

        private string GetItemImage(string itemType, int itemId)
        {
            string imageName = null;

            switch (itemType)
            {
                case "Game":
                    imageName = sqlContext.Games.FirstOrDefault(g => g.GameId == itemId)?.GameImage;
                    break;
                case "Movie":
                    imageName = sqlContext.Movies.FirstOrDefault(m => m.MovieId == itemId)?.MovieImage;
                    break;
                case "Album":
                    imageName = sqlContext.Albums.FirstOrDefault(a => a.AlbumId == itemId)?.AlbumImage;
                    break;
            }

            if (string.IsNullOrEmpty(imageName))
            {
                return "default-image.jpg";
            }

            return $"{itemType.ToLower()}s/{imageName}";
        }

        private string GetPlatformName(string itemType, int? platformId)
        {
            if (itemType != "Game" || !platformId.HasValue)
            {
                return "N/A";
            }

            var platform = sqlContext.Platforms.FirstOrDefault(p => p.PlatformId == platformId.Value);
            return platform?.PlatformName ?? "N/A";
        }

        [HttpPost]
        public IActionResult AddToCart(string itemType, int itemId, int? platformId)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var cart = sqlContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value };
                sqlContext.Carts.Add(cart);
            }

            decimal price = 0;
            switch (itemType)
            {
                case "Game":
                    var game = sqlContext.Games.Find(itemId);
                    price = game?.GamePrice ?? 0;
                    break;
                case "Movie":
                    var movie = sqlContext.Movies.Find(itemId);
                    price = movie?.MoviePrice ?? 0;
                    break;
                case "Album":
                    var album = sqlContext.Albums.Find(itemId);
                    price = album?.AlbumPrice ?? 0;
                    break;
            }

            if (price == 0)
            {
                TempData["ErrorMessage"] = "Invalid item selected.";
                return RedirectToAction("Index", "Home");
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(item => item.ItemType == itemType && item.ItemId == itemId && item.PlatformId == platformId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ItemType = itemType,
                    ItemId = itemId,
                    PlatformId = platformId,
                    Quantity = 1,
                    Price = price
                });
            }

            sqlContext.SaveChanges();

            return RedirectToAction("CartView");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = sqlContext.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (cartItem != null)
            {
                sqlContext.CartItems.Remove(cartItem);
                sqlContext.SaveChanges();
            }

            return RedirectToAction("CartView");
        }

        [HttpPost]
        public IActionResult Checkout(string phone, string country, string city, string address)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Retrieve the user's cart
            var cart = sqlContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("CartView");
            }

            var userAddress = sqlContext.UserAddresses.FirstOrDefault(ua => ua.UserId == userId);
            if (userAddress == null)
            {
                userAddress = new UserAddress
                {
                    UserId = userId.Value,
                    Phone = phone,
                    Country = country,
                    City = city,
                    Address = address
                };
                sqlContext.UserAddresses.Add(userAddress);
            }
            else
            {
                userAddress.Phone = phone;
                userAddress.Country = country;
                userAddress.City = city;
                userAddress.Address = address;
            }

            // ✅ Create the order
            var order = new Order
            {
                UserId = userId.Value,
                Phone = phone,
                Country = country,
                City = city,
                Address = address,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.CartItems.Sum(item => item.Price * item.Quantity),
                Fulfilled = false
            };

            sqlContext.Orders.Add(order);
            sqlContext.SaveChanges(); // Save to generate OrderId

            // ✅ Copy cart items to OrderItems
            var orderItems = cart.CartItems.Select(cartItem => new OrderItem
            {
                OrderId = order.OrderId, // Link to the new order
                ItemType = cartItem.ItemType,
                ItemId = cartItem.ItemId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price
            }).ToList();

            sqlContext.OrderItems.AddRange(orderItems);

            // ✅ Clear the cart
            sqlContext.CartItems.RemoveRange(cart.CartItems);

            sqlContext.SaveChanges(); // Final save

            TempData["SuccessMessage"] = "Your order has been placed successfully!";

            return RedirectToAction("CartView");
        }


        //Search

        public IActionResult SearchResults(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new SearchResultsViewModel());
            }

            var model = new SearchResultsViewModel
            {
                Movies = sqlContext.Movies
                    .Where(m => m.MovieTitle.Contains(query))
                    .ToList(),

                Albums = sqlContext.Albums
                    .Where(a => a.AlbumTitle.Contains(query))
                    .ToList(),

                Games = sqlContext.Games
                    .Where(g => g.GameName.Contains(query))
                    .ToList()
            };

            return View(model);
        }
    }
}
