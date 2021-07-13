using company.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly CompanyDBContext _dbContext;
        public CompanyRepository(CompanyDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Company RegisterCompany(Company company)
        {
            _dbContext.Companies.Add(company);
            _dbContext.CompanyExchanges.AddRange(company.CompanyExchange);
            _dbContext.SaveChanges();
            return company;
        }
        public Company DeleteCompany(string companyCode)
        {
            var comp = this.GetCompanyByCode(companyCode);
            _dbContext.Companies.Remove(comp);
            var compExchange=GetCompanyExchangeByCode(companyCode);
            _dbContext.CompanyExchanges.RemoveRange(compExchange);
            _dbContext.SaveChanges();
            return comp;
        }
        private List<CompanyExchange> GetCompanyExchangeByCode(string companyCode){
            return _dbContext.CompanyExchanges.Where(x=>x.CompanyCode==companyCode).ToList();
        }
        public Company GetCompanyByCode(string companyCode)
        {
            return _dbContext.Companies.Include(x => x.CompanyExchange).ToList().Find(x=> x.CompanyCode.Equals(companyCode,StringComparison.OrdinalIgnoreCase));
        }
        public IReadOnlyList<Company> GetAllCompanies()
        {
            return _dbContext.Companies.Include(x=>x.CompanyExchange).ToList();
        }
        public bool IsCompanyExists(string companyCode)
        {
            var company= _dbContext.Companies.Include(x => x.CompanyExchange).ToList().Find(x=> x.CompanyCode.Equals(companyCode,StringComparison.OrdinalIgnoreCase));
            var comp = GetCompanyByCode(companyCode);
            return comp != null;
        }

        public bool IsExchangeExists(List<string> exchanges){
            int queruCount= _dbContext.Exchanges.Where(x=>exchanges.Contains(x.ExchangeName.ToLower())).Count();
            return exchanges.Count == queruCount;
        }

        public void UpdateCompanyExchangeCurrentPrice(string companyCode,string  exchangeName,decimal currentStockPrice){
            var item=_dbContext.CompanyExchanges.Where(x=>x.CompanyCode==companyCode && x.ExchangeName==exchangeName).FirstOrDefault();
            if(item!=null){
                item.StockPrice=currentStockPrice;
               _dbContext.CompanyExchanges.Update(item);
               _dbContext.SaveChanges();
            }
        }
        
        /*
        public void UpdateCompanyStock(Stock stock)
        {
            var comp = _dbContext.Companies.Where(x => x.CompanyCode.Equals(stock.CompanyCode)).FirstOrDefault();
            
            if (comp!=null)
            {
                stock.StockId = 0;
                stock.Company = comp;
                _dbContext.Stocks.Add(stock);
                _dbContext.SaveChanges();
            }           
        }
        */
    }
}
