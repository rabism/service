using company.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Repository
{
    public interface ICompanyRepository
    {
        Company RegisterCompany(Company company);
        Company DeleteCompany(string companyCode);
        Company GetCompanyByCode(string companyCode);
        IReadOnlyList<Company> GetAllCompanies();
        bool IsCompanyExists(string companyCode);
        bool IsExchangeExists(List<string> exchanges);
        void UpdateCompanyExchangeCurrentPrice(string companyCode,string  exchangeName,decimal currentStockPrice);
    }
}
