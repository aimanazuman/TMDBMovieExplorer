using Newtonsoft.Json;

namespace TMDBMovieExplorer.Models
{
    public class MovieResponse
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("results")]
        public List<Movie> Results { get; set; } = new List<Movie>();

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }
    }

    public class Movie
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonProperty("poster_path")]
        public string? PosterPath { get; set; }

        [JsonProperty("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("genre_ids")]
        public List<int> GenreIds { get; set; } = new List<int>();

        // Helper properties for display
        public string FullPosterUrl => !string.IsNullOrEmpty(PosterPath)
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
            : "/images/no-poster.jpg";

        public string FullBackdropUrl => !string.IsNullOrEmpty(BackdropPath)
            ? $"https://image.tmdb.org/t/p/w1280{BackdropPath}"
            : "/images/no-backdrop.jpg";

        public string FormattedReleaseDate => DateTime.TryParse(ReleaseDate, out var date)
            ? date.ToString("MMM dd, yyyy")
            : "Unknown";
    }

    public class MovieDetails : Movie
    {
        [JsonProperty("runtime")]
        public int? Runtime { get; set; }

        [JsonProperty("budget")]
        public long Budget { get; set; }

        [JsonProperty("revenue")]
        public long Revenue { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; } = new List<Genre>();

        [JsonProperty("production_companies")]
        public List<ProductionCompany> ProductionCompanies { get; set; } = new List<ProductionCompany>();

        public string FormattedRuntime => Runtime.HasValue
            ? $"{Runtime / 60}h {Runtime % 60}m"
            : "Unknown";

        public string FormattedBudget => Budget > 0
            ? Budget.ToString("C0")
            : "Unknown";
    }

    public class Genre
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class ProductionCompany
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("logo_path")]
        public string? LogoPath { get; set; }
    }

    public class ApiErrorResponse
    {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("status_message")]
        public string StatusMessage { get; set; } = string.Empty;

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}