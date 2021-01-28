using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class DoExpressionExpectedException : SyntaxErrorException
    {
        public DoExpressionExpectedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Do requires a Concept name or expression resolving into an attribute or set list"); 
        }
  
    }
}
