using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace company.Entity
{
  public class Exchange{
      [Key]
      [Required]
      public string ExchangeName{get;set;}
      [Required]
      public string ExchangeDescription {get;set;}
       public virtual ICollection<CompanyExchange> CompanyExchange { get; set; }
  }
}