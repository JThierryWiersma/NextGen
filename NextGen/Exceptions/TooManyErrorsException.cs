using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class TooManyErrorsException : ApplicationException
    {
        public TooManyErrorsException()
            : base()
        {
        }
        public override string Message
        {
            get
            {
                return String.Format("Too many error encountered. Generation aborted");
            }
        }

    }
}
