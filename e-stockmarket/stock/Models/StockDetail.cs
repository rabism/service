using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace stock.Models
{
    public class StockDetail
    {
        public string StockId { get; set; }
        public decimal StockPrice { get; set; }
        public DateTime StockDateTime { get; set; } = DateTime.Now;
        public string CompanyCode { get; set; }
        public string ExchangeName {get;set;}
        public string Time {get;set;}

    }
}