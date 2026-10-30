using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Exceptions
{
    public class EmptyCartException : Exception
    {
        public EmptyCartException(string message) : base(message) { }
    }
}
