using MediatR;
using stock.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace stock.Queries
{
    public class CompanySearchByCodeAndExchageQuery : IRequest<IReadOnlyList<Company>>
    {
        public string CompanyCode { get; set; }
        public string ExchangeName {get;set;}
        
    }

    public class CompanySearchByCodeAndExchageQueryHandler : IRequestHandler<CompanySearchByCodeAndExchageQuery, IReadOnlyList<Company>>
        {
            private readonly IStockDbContext _context;
            public CompanySearchByCodeAndExchageQueryHandler(IStockDbContext context)
            {
                _context = context;
            }
            public async Task<IReadOnlyList<Company>> Handle(CompanySearchByCodeAndExchageQuery query, CancellationToken cancellationToken)
            {
                return await _context.Company.FindAsync(x => x.CompanyCode.Equals(query.CompanyCode) && x.Exchanges.Any(y=> y.ExchangeName==query.ExchangeName) ).Result.ToListAsync();
            }
        }
}
