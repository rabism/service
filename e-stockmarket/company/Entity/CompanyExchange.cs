using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace company.Entity
{
    public class CompanyExchange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int  CompanyExchangeId {get;set;}
        public string CompanyCode{get;set;}
        [Required]
        public string ExchangeName {get;set;}
        [Column(TypeName = "decimal(18,4)")]
        public decimal StockPrice {get;set;}
        public virtual Company Company { get; set; }
        public virtual Exchange Exchange { get; set; }
    }
}
