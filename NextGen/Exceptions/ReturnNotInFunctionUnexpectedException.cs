using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{

    class ReturnNotInFunctionUnexpectedException : SyntaxErrorException
    {
        public ReturnNotInFunctionUnexpectedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Return statement is only allowed in Function body");
        }
    }

 
}
