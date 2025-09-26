using Microsoft.AspNetCore.Mvc;
using TMDBMovieExplorer.Services;
using TMDBMovieExplorer.Models;

namespace TMDBMovieExplorer.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ITMDBService _tmdbService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ITMDBService tmdbService, ILogger<MoviesController> logger)
        {
            _tmdbService = tmdbService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string category = "popular", int page = 1)
        {
            try
            {
                MovieResponse movies = category.ToLower() switch
                {
                    "top-rated" => await _tmdbService.GetTopRatedMoviesAsync(page),
                    "now-playing" => await _tmdbService.GetNowPlayingMoviesAsync(page),
                    _ => await _tmdbService.GetPopularMoviesAsync(page)
                };

                ViewBag.Category = category;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = Math.Min(movies.TotalPages, 500); // TMDB limits to 500 pages

                return View(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movies for category: {Category}", category);
                ViewBag.ErrorMessage = "Unable to load movies. Please try again later.";
                return View(new MovieResponse());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var searchResults = await _tmdbService.SearchMoviesAsync(query, page);

                ViewBag.SearchQuery = query;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = Math.Min(searchResults.TotalPages, 500);

                return View(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies with query: {Query}", query);
                ViewBag.ErrorMessage = "Unable to search movies. Please try again later.";
                ViewBag.SearchQuery = query;
                return View(new MovieResponse());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var movie = await _tmdbService.GetMovieDetailsAsync(id);

                if (movie == null)
                {
                    return NotFound("Movie not found.");
                }

                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movie details for ID: {MovieId}", id);
                ViewBag.ErrorMessage = "Unable to load movie details. Please try again later.";
                return View();
            }
        }
    }
}