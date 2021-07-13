
namespace stock.Services
{
    public interface IStockUpdateSenderService
    {
        void SendAddStock(string companyCode,string exchangeName,decimal currentStockPrice);
    }
}
