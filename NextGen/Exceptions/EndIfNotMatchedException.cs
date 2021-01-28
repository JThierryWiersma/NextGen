using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class EndIfNotMatchedException : SyntaxErrorException
    {
        public EndIfNotMatchedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("EndIf not matched with If, ElseIf or Else"); 
        }
  
    }
}
