using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models
{
    /*
    * Custom Exception thrown when Company being requested does not exist
    */

    public class CompanyNotFoundException : ApplicationException
    {
        public CompanyNotFoundException() { }
        public CompanyNotFoundException(string message) : base(message) { }
    }
}
