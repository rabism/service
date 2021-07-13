using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stock.Exceptions
{
    /*
     * Custom Exception thrown when Stock being requested does not exist
     */

    public class StockNotFoundException : ApplicationException
    {
        public StockNotFoundException() { }
        public StockNotFoundException(string message) : base(message) { }
    }
}
