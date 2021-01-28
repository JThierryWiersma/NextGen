using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ElseNotMatchedException : SyntaxErrorException
    {
        public ElseNotMatchedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Else not matched with If or ElseIf"); 
        }
  
    }
}