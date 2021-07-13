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
    public class CompanySearchByCodeQuery : IRequest<IReadOnlyList<Company>>
    {
        public string CompanyCode { get; set; }
        
    }

    public class CompanySearchByCodeQueryHandler : IRequestHandler<CompanySearchByCodeQuery, IReadOnlyList<Company>>
        {
            private readonly IStockDbContext _context;
            public CompanySearchByCodeQueryHandler(IStockDbContext context)
            {
                _context = context;
            }
            public async Task<IReadOnlyList<Company>> Handle(CompanySearchByCodeQuery query, CancellationToken cancellationToken)
            {
                return await _context.Company.FindAsync(x => x.CompanyCode.Equals(query.CompanyCode)).Result.ToListAsync();
            }
        }
}
