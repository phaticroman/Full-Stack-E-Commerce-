using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MaleFashion.Application.Exceptions
{
   
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string message) : base(message) { }
    }
}
