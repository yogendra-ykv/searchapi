using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SearchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Searches for items matching the query.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="sortBy">Sort criteria (e.g., relevance or date).</param>
        /// <param name="startDate">Start date for filtering results.</param>
        /// <param name="endDate">End date for filtering results.</param>
        /// <returns>Search results matching the query.</returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] string sortBy = "relevance",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
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

            return Ok(await results.ToListAsync());
        }
    }
}
