
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using stock.Entity;
using stock.Exceptions;
using stock.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using stock.Commands;
using stock.Queries;
using stock.Services;

namespace stock.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStockUpdateSenderService stockUpdateSenderService;
       private readonly ILogger<StockController> _logger;
       public StockController(ILogger<StockController> logger,IMediator mediator,IStockUpdateSenderService _stockUpdateSenderService)
        {
            _logger = logger;
            _mediator=mediator;
            stockUpdateSenderService=_stockUpdateSenderService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{companycode}")]
        public async Task<IActionResult> Post([FromBody] StockDetail stock, string companycode)
        {
            try
            {

                var company = await _mediator.Send(new CompanySearchByCodeQuery()
                {
                    CompanyCode = companycode

                });

                if (company == null || !company.Any())
                {
                    return BadRequest($"Company code {companycode} not registered!!");
                }

                var companyExchange = await _mediator.Send(new CompanySearchByCodeAndExchageQuery()
                {
                    CompanyCode = companycode,
                    ExchangeName = stock.ExchangeName

                });

                if (companyExchange == null || !companyExchange.Any())
                {
                    return BadRequest($"Exchange {stock.ExchangeName} not listed!!");
                }

                Stock _stock = MapToEntity(stock, companycode);
                AddStockCommand command = new AddStockCommand
                {
                    Stock = _stock
                };
                await _mediator.Send(command);
                Task senderService = new Task(() => stockUpdateSenderService.SendAddStock(_stock.CompanyCode, _stock.ExchangeName, _stock.StockPrice));
                senderService.Start();
                return CreatedAtAction(nameof(Post), companycode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Adding the stock: {ex.Message}");
                return StatusCode(500);
            }
        }

        private Stock MapToEntity(StockDetail stock,string companyCode)
        {
            return new Stock
            {
                StockPrice = stock.StockPrice,
                ExchangeName=stock.ExchangeName,
                CompanyCode= companyCode
            };
           
        }

        [HttpGet]
        [Route("{companycode}/{startdate}/{enddate}")]
        public async Task<IActionResult> Get(string companycode, DateTime startdate, DateTime enddate)
        {
            try
            {
                _logger.LogInformation($"Getting stock of {companycode} for the time-period {startdate} - {enddate}");
                var stocks = await _mediator.Send(new StockSearchByDateQuery()
                {
                    CompanyCode = companycode,
                    StartDate = startdate,
                    EndDate = enddate
                });
                var result=stocks.Select(x=>new StockDetailResponse{
                    StockId=x.StockId,
                    StockPrice=x.StockPrice,
                    CompanyCode=x.CompanyCode,
                    StockDateTime=x.StockDateTime,
                    ExchangeName=x.ExchangeName,
                    Time=x.StockDateTime.ToString("HH:mm")

                }).ToList();
                return Ok(result);
            }
            catch (StockNotFoundException pnf)
            {
                _logger.LogInformation($"Required stock does not exist");
                return NotFound(pnf.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in fetching data: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
