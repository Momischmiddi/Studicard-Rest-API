using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTApi.Exceptions
{
    public class IDDoesNotExistException : Exception
    {
        public IDDoesNotExistException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}
