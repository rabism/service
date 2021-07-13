using MediatR;
using MongoDB.Driver;
using stock.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace stock.Queries
{
    public class StockSearchByDateQuery : IRequest<IReadOnlyList<Stock>>
    {
        public string CompanyCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
       
    }

     public class StockSearchByDateQueryHandler : IRequestHandler<StockSearchByDateQuery, IReadOnlyList<Stock>>
        {
            readonly IStockDbContext _context;
            public StockSearchByDateQueryHandler(IStockDbContext context)
            {
                _context = context;
            }
            public async Task<IReadOnlyList<Stock>> Handle(StockSearchByDateQuery query, CancellationToken cancellationToken)
            {
                return await _context.Stocks.FindAsync(x => x.CompanyCode.Equals(query.CompanyCode) && x.StockDateTime > query.StartDate && x.StockDateTime < query.EndDate).Result.ToListAsync();
            }
        }
}
