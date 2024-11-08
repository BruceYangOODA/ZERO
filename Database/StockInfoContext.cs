using Microsoft.EntityFrameworkCore;
using ZERO.Database.StockInfo;

namespace ZERO.Database
{
    public class StockInfoContext : DbContext
    {
        public StockInfoContext(DbContextOptions<StockInfoContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }

        public DbSet<QuoteInfo> QuoteInfos { get; set; }
    }
}
