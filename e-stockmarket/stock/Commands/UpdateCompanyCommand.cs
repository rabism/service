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
    public class UpdateCompanyCommand : IRequest<Company>
    {
        public Company Company { get; set; }

        
    }
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Company>
    {

        readonly ILogger<UpdateCompanyCommandHandler> logger;
        private readonly IStockDbContext _context;
        public UpdateCompanyCommandHandler(IStockDbContext context, ILogger<UpdateCompanyCommandHandler> _logger)
        {
            _context = context;
            logger = _logger;
        }
        public async Task<Company> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _context.Company.InsertOneAsync(command.Company);
                logger.LogInformation("company added success!");
                return command.Company;
            }
            catch (Exception ex)
            {
                logger.LogError("Eror occurs on adding compny" + ex.Message);
                throw ex;
            }

        }
    }
}
