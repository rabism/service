using company.Entity;
using company.Models;
using company.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace company.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        readonly ICompanyService service;
        readonly ILogger<CompanyController> _logger;
        readonly ICompanyUpdateSenderService updateSenderService;
        public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger, ICompanyUpdateSenderService _updateSenderService)
        {
            service = companyService;
            _logger = logger;
            updateSenderService=_updateSenderService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult Post([FromBody] CompanyDetail company)
        {
            try
            {
                _logger.LogInformation($"Adding a new Company {company}");
                Company _company = MapToCompany(company);
                service.Register(_company);
                _logger.LogInformation($"sending message to queue");
                Task senderService = new Task(() => updateSenderService.SendAddCompany(_company));
                senderService.Start();
                return Created("", company);
            }
            catch (CompanyAlreadyExistsException pae)
            {
                _logger.LogInformation($"This company already exists {company.CompanyCode}");
                return Conflict(pae.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Adding the company: {ex.Message}");
                return StatusCode(500);
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        [Route("{companycode}")]
        public IActionResult Delete(string companycode)
        {
            try
            {
                _logger.LogInformation($"Removing the company {companycode}");
                service.Delete(companycode);
                _logger.LogInformation($"sending message to queue");
                Task senderService = new Task(() => updateSenderService.SendDeleteCompany(companycode));
                senderService.Start();
                return NoContent();
            }
            catch (CompanyNotFoundException pnf)
            {
                _logger.LogInformation("Company trying to remove does not exist");
                return NotFound(pnf.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in deleting the company: {ex.Message}");
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("{companycode}")]
        public IActionResult Get(string companycode)
        {
            try
            {
                _logger.LogInformation($"Getting Company Details for {companycode}");
                return Ok(MapToCompanyModel(service.GetCompany(companycode)));
            }
            catch (CompanyNotFoundException pnf)
            {
                _logger.LogInformation($"Required CompanyCode does not exist");
                return NotFound(pnf.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in fetching data: {ex.Message}");
                return StatusCode(500);
            }
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation($"Getting All Companies");
                var comps = service.GetAllCompanies();
                var companies = new List<CompanyDetail>();
                if (comps != null && comps.Count > 0)
                {
                    foreach (var comp in comps)
                    {
                        companies.Add(MapToCompanyModel(comp));
                    }
                    
                }
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in fetching data: {ex.Message}");
                return StatusCode(500);
            }
        }
        Company MapToCompany(CompanyDetail company)
        {
            Company comp = new Company
            {
                CompanyCEO = company.CompanyCEO,
                CompanyCode = company.CompanyCode,
                CompanyName = company.CompanyName,
                CompanyTurnOver = company.CompanyTurnOver,
                Website = company.Website,
                CompanyExchange=company.Exchange.Select(x=>new CompanyExchange{
                 ExchangeName=x.ExchangeName,
                 CompanyCode=company.CompanyCode,
                 StockPrice=x.StockPrice
                }).ToList()
                
            };
            return comp;
        }
        CompanyDetail MapToCompanyModel(Company company)
        {
            CompanyDetail comp = new CompanyDetail
            {
                CompanyCEO = company.CompanyCEO,
                CompanyCode = company.CompanyCode,
                CompanyName = company.CompanyName,
                CompanyTurnOver = company.CompanyTurnOver,
                Website = company.Website,
                Exchange=company.CompanyExchange.Select(x=>new ExchangeDetail{
                    ExchangeName=x.ExchangeName,
                    StockPrice=x.StockPrice
                }).ToList()
            };
            return comp;
        }
    }

}
