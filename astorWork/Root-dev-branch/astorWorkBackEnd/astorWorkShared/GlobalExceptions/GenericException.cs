using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkShared.GlobalExceptions
{
    public class GenericException : Exception
    {
        public int Code { get; set; }
        public GenericException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
