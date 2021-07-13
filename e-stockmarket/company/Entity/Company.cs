using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace company.Entity
{
    public class Company
    {
        [Key]
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyCEO { get; set; }
        [Required]
        [Range(10, int.MaxValue, ErrorMessage = "Company Turnover must be greater than 10Cr.")]
        public decimal CompanyTurnOver { get; set; }
        [Required]
        public string Website { get; set; }
        public virtual ICollection<CompanyExchange> CompanyExchange { get; set; }

    }
}
