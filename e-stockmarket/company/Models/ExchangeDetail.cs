using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models
{
   public class ExchangeDetail{
        
        [Required]
        public string ExchangeName { get; set; }
        [Required]
        [RegularExpression(@"(^[0-9]+[.][0-9]{2,}$)", ErrorMessage = "Stock price must be a fractional value.")]
        public decimal StockPrice { get; set; }
   }
}