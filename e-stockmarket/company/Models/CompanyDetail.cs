using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using company.Validator;
namespace company.Models
{
   public class CompanyDetail
   {
        [Required]
        [UniqueCompanyCode]
        public string CompanyCode { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyCEO { get; set; }
        [Required]
        [Range(10, (double)decimal.MaxValue, ErrorMessage = "Company Turnover must be greater than 10Cr.")]
        public decimal CompanyTurnOver { get; set; }
        [Required]
        public string Website { get; set; }
        [Required]
        [ExchangeNotListed]
        public List<ExchangeDetail> Exchange {get;set;}

   }
}