using Microsoft.EntityFrameworkCore;
namespace company.Entity
{

    public class CompanyDBContext : DbContext
    {
        public CompanyDBContext(DbContextOptions<CompanyDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Exchange> Exchanges{get;set;}
        public DbSet<CompanyExchange> CompanyExchanges { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exchange>().HasData(new Exchange {
                ExchangeName="NSE",
                ExchangeDescription="NSE"
            });
            modelBuilder.Entity<Exchange>().HasData(new Exchange {
                ExchangeName="BSE",
                ExchangeDescription="BSE"
            });
            
        }
    }
}
