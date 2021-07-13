using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stock.Exceptions
{
    /*
     * Custom Exception thrown when Stock with existing CompanyCode is being added
    */

    public class CompanyNotRegisteredExaception : ApplicationException
    {
        public CompanyNotRegisteredExaception() { }
        public CompanyNotRegisteredExaception(string message) : base(message) { }
    }
}
