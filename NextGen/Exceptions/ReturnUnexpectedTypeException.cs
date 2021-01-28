using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ReturnUnexpectedTypeException : SyntaxErrorException
    {
        public ReturnUnexpectedTypeException(SourceCodeContext linenr, string expectedtype, string actualtype)
            : base(linenr)
        {
            _msg = String.Format("Function is declared to return a {0}. Return statement expression should not evaluate to '{1}'.", expectedtype, actualtype);  
        }
    }
}
