using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ElseIfNotMatchedException : SyntaxErrorException
    {
        public ElseIfNotMatchedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("ElseIf not matched with If or ElseIf"); 
        }
  
    }
}
