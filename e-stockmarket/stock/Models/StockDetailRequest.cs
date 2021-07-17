using System.ComponentModel.DataAnnotations;
namespace stock.Models
{
    public class StockDetailRequest
    {
       
        [Required]
        public decimal StockPrice { get; set; }
        [Required]
        public string ExchangeName {get;set;}
        
    }
}
