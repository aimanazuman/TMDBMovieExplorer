using TMDBMovieExplorer.Models;
using Newtonsoft.Json;
using System.Web;

namespace TMDBMovieExplorer.Services
{
    public interface ITMDBService
    {
        Task<MovieResponse> GetPopularMoviesAsync(int page = 1);
        Task<MovieResponse> SearchMoviesAsync(string query, int page = 1);
        Task<MovieDetails?> GetMovieDetailsAsync(int movieId);
        Task<MovieResponse> GetTopRatedMoviesAsync(int page = 1);
        Task<MovieResponse> GetNowPlayingMoviesAsync(int page = 1);
    }

    public class TMDBService : ITMDBService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";

        public TMDBService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["TMDB:ApiKey"] ?? throw new InvalidOperationException("TMDB API key not found");
        }

        public async Task<MovieResponse> GetPopularMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/movie/popular?api_key={_apiKey}&page={page}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MovieResponse>(json) ?? new MovieResponse();
                }

                return await HandleErrorResponse(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error fetching popular movies: {ex.Message}", ex);
            }
        }

        public async Task<MovieResponse> SearchMoviesAsync(string query, int page = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return new MovieResponse();

                var encodedQuery = HttpUtility.UrlEncode(query);
                var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={encodedQuery}&page={page}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MovieResponse>(json) ?? new MovieResponse();
                }

                return await HandleErrorResponse(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error searching movies: {ex.Message}", ex);
            }
        }

        public async Task<MovieDetails?> GetMovieDetailsAsync(int movieId)
        {
            try
            {
                var url = $"{_baseUrl}/movie/{movieId}?api_key={_apiKey}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MovieDetails>(json);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                await HandleErrorResponse(response);
                return null;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error fetching movie details: {ex.Message}", ex);
            }
        }

        public async Task<MovieResponse> GetTopRatedMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/movie/top_rated?api_key={_apiKey}&page={page}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MovieResponse>(json) ?? new MovieResponse();
                }

                return await HandleErrorResponse(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error fetching top rated movies: {ex.Message}", ex);
            }
        }

        public async Task<MovieResponse> GetNowPlayingMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/movie/now_playing?api_key={_apiKey}&page={page}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MovieResponse>(json) ?? new MovieResponse();
                }

                return await HandleErrorResponse(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error fetching now playing movies: {ex.Message}", ex);
            }
        }

        private async Task<MovieResponse> HandleErrorResponse(HttpResponseMessage response)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorContent);
                throw new HttpRequestException($"TMDB API Error: {errorResponse?.StatusMessage ?? "Unknown error"}");
            }
            catch (JsonException)
            {
                throw new HttpRequestException($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }
}