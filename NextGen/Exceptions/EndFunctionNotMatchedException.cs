using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class EndFunctionNotMatchedException : SyntaxErrorException
    {
        public EndFunctionNotMatchedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("EndFunction not matched with Function"); 
        }
  
    }
}
