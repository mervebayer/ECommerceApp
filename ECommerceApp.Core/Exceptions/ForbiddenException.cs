using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Core.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Access to this resource is forbidden."): base(message){ 
        } 
    }
}
