using System.ComponentModel.DataAnnotations;
using System;
namespace stock.Models
{
    public class StockSummary
    {
        public decimal MinPrice{get;set;}
        public decimal MaxPrice {get;set;}
        public decimal AvgPrice {get;set;}
        public string ExchangeName {get;set;}
    }
}