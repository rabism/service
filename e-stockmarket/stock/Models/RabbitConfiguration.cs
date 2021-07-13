using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace stock.Models
{
   public class RabbitConfiguration{
        
        public string Hostname { get; set; }

        public string AddCompanyQueueName { get; set; }

        public string DeleteCompanyQueueName { get; set; }

        public string AddStockQueueName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
   }
}