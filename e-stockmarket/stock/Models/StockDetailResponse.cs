using System.ComponentModel.DataAnnotations;
using System;
namespace stock.Models
{
    public class StockDetailResponse
    {
        [Required]
        public string StockId { get; set; }
        [Required]
        public decimal StockPrice { get; set; }
        [Required]
        public DateTime StockDateTime { get; set; } = DateTime.Now;
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string ExchangeName {get;set;}
        [Required]
        public string Time {get;set;}
        
    }
}