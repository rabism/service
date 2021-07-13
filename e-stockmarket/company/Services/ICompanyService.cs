using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using company.Entity;

namespace company.Services
{
    public interface ICompanyService
    {
        Company Register(Company company);
        Company Delete(string companyCode);
        Company GetCompany(string companyCode);
       // void UpdateCompanyStock(Stock stock);
        IReadOnlyList<Company> GetAllCompanies();
         bool IsExchangeExists(List<string> exchanges);
         bool IsCompanyExists(string companyCode);
        void UpdateCompanyExchangeCurrentPrice(string companyCode,string  exchangeName,decimal currentStockPrice);
    }
}
