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
    public class StockSearchByCompanyCodeQuery : IRequest<IReadOnlyList<Stock>>
    {
        public string CompanyCode { get; set; }
        
    }

    public class StockSearchByCompanyCodeQueryHandler : IRequestHandler<StockSearchByCompanyCodeQuery, IReadOnlyList<Stock>>
        {
            private readonly IStockDbContext _context;
            public StockSearchByCompanyCodeQueryHandler(IStockDbContext context)
            {
                _context = context;
            }
            public async Task<IReadOnlyList<Stock>> Handle(StockSearchByCompanyCodeQuery query, CancellationToken cancellationToken)
            {
                return await _context.Stocks.FindAsync(x => x.CompanyCode.Equals(query.CompanyCode)).Result.ToListAsync();
            }
        }
}
