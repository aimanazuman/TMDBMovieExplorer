using Microsoft.AspNetCore.Mvc;
using TMDBMovieExplorer.Models;
using TMDBMovieExplorer.Services;
using System.Diagnostics;

namespace TMDBMovieExplorer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITMDBService _tmdbService;

        public HomeController(ILogger<HomeController> logger, ITMDBService tmdbService)
        {
            _logger = logger;
            _tmdbService = tmdbService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get featured movies for homepage
                var popularMovies = await _tmdbService.GetPopularMoviesAsync(1);
                var topRatedMovies = await _tmdbService.GetTopRatedMoviesAsync(1);
                var nowPlayingMovies = await _tmdbService.GetNowPlayingMoviesAsync(1);

                ViewBag.PopularMovies = popularMovies.Results.Take(6).ToList();
                ViewBag.TopRatedMovies = topRatedMovies.Results.Take(6).ToList();
                ViewBag.NowPlayingMovies = nowPlayingMovies.Results.Take(6).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading homepage data");
                ViewBag.ErrorMessage = "Unable to load featured movies.";
            }

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}