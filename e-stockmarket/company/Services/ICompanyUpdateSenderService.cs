using company.Models;
using company.Entity;
using company.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Services
{
    public interface ICompanyUpdateSenderService
    {
            void SendAddCompany(Company company);
             void SendDeleteCompany(string companyCode);
    }
}