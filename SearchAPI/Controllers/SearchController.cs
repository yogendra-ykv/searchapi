namespace SearchAPI.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IMemoryCache _cache;

        public SearchController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        /// <summary>
        /// Searches for items matching the query
        /// </summary>
        /// <param name="query">The search query string</param>
        /// <param name="sortBy">Sort criteria (e.g., relevance or date)</param>
        /// <param name="startDate">Start date for filtering results</param>
        /// <param name="endDate">End date for filtering results</param>
        /// <returns>Search results matching the query</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search(
    [FromQuery] string query,
    [FromQuery] string sortBy = "relevance",
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
        {
            var cacheKey = $"SearchResults-{query}-{sortBy}-{startDate}-{endDate}";
            if (_cache.TryGetValue(cacheKey, out List<SearchHistory> cachedResults))
            {
                return Ok(cachedResults); // Return cached results if available
            }

            var results = _context.SearchHistories.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                results = results.Where(r => EF.Functions.Like(r.Query, $"%{query}%"));
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                results = results.Where(r => r.CreatedDate >= startDate && r.CreatedDate <= endDate);
            }

            results = sortBy switch
            {
                "date" => results.OrderByDescending(r => r.CreatedDate),
                _ => results.OrderByDescending(r => r.Id) // Default relevance sorting
            };

            var finalResults = await results.ToListAsync();
            _cache.Set(cacheKey, finalResults, TimeSpan.FromMinutes(5)); // Cache results for 5 minutes

            return Ok(finalResults);
        }
    }
}
