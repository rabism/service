using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace stock.Models
{
    public class StockDetailResponse
    {
        
        public List<StockSummary> Summary {get;set;}
        public List<StockDetail> Detail {get;set;}
        
    }
}