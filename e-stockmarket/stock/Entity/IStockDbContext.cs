using MongoDB.Driver;

namespace stock.Entity
{
    public interface IStockDbContext
    {
        IMongoCollection<Stock> Stocks { get; }

        IMongoCollection<Company> Company {get;}
    }
}