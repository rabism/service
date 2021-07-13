using MediatR;
using stock.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace stock.Commands
{
    public class DeleteCompanyStockCommand : IRequest<bool>
    {
        public string companyCode{get;set;}

        
    }
    public class DeleteCompanyStockCommandHandler : IRequestHandler<DeleteCompanyStockCommand,bool>
        {
            private readonly IStockDbContext _context;
            private readonly ILogger<DeleteCompanyStockCommandHandler> logger;
            public DeleteCompanyStockCommandHandler(IStockDbContext context,ILogger<DeleteCompanyStockCommandHandler> _logger)
            {
                _context = context;
                logger=_logger;
            }
            public async Task<bool> Handle(DeleteCompanyStockCommand command, CancellationToken cancellationToken)
            {
               try
               {
                await _context.Stocks.DeleteManyAsync(x => x.CompanyCode.Equals(command.companyCode));
                await _context.Company.DeleteManyAsync(x => x.CompanyCode.Equals(command.companyCode));
                logger.LogInformation("Successfully delete the record");
                return true;
               }
               catch(Exception ex)
               {
                   logger.LogError("Error occurs on delete stock "+ex.Message);
                   return false;
               }
              
            }
        }
}
