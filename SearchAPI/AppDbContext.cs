using Microsoft.EntityFrameworkCore;

namespace SearchAPI
{
    

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<SearchHistory> SearchHistories { get; set; }
    }
    public class SearchHistory
    {
        public int Id { get; set; }
        public string Query { get; set; }
        public string Result { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
    }
}
