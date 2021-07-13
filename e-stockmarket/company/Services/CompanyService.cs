using company.Models;
using company.Entity;
using company.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Services
{
    public class CompanyService : ICompanyService
    {
        readonly ICompanyRepository repository;
        public CompanyService(ICompanyRepository companyRepository)
        {
            repository = companyRepository;
        }
        public Company Register(Company company)
        {
            if (IsCompanyExists(company.CompanyCode))
            {
                throw new CompanyAlreadyExistsException($"Company {company.CompanyCode} Already Exists !!!");
            }
            else
            {
                return repository.RegisterCompany(company);
            }
        }
        public Company Delete(string companyCode)
        {
            var existingCompany = repository.GetCompanyByCode(companyCode);
            if (existingCompany == null)
            {
                throw new CompanyNotFoundException($"Company with code {companyCode} Does Not Exist !!!");
            }
            else
            {
                return repository.DeleteCompany(companyCode);
            }
        }
        public Company GetCompany(string companyCode)
        {
            var comp = repository.GetCompanyByCode(companyCode);
            if (comp == null)
                throw new CompanyNotFoundException("Company with this company code does not exist");
            return comp;
        }
       public IReadOnlyList<Company> GetAllCompanies()
        {
            return repository.GetAllCompanies();
        }
       public  bool IsExchangeExists(List<string> exchanges){
           return repository.IsExchangeExists(exchanges);
        }

        public bool IsCompanyExists(string companyCode)
        {
           return repository.IsCompanyExists(companyCode);
        }

        public void UpdateCompanyExchangeCurrentPrice(string companyCode,string  exchangeName,decimal currentStockPrice)
        {
            repository.UpdateCompanyExchangeCurrentPrice(companyCode,exchangeName,currentStockPrice);
        }
        /*
        public void UpdateCompanyStock(Stock stock)
        {
            var comp = repository.GetCompanyByCode(stock.CompanyCode);
            if (comp == null)
                throw new CompanyNotFoundException("Company with this code not found");
            repository.UpdateCompanyStock(stock);
        }
        */
    }
}
