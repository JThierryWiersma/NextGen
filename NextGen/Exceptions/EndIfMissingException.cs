using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class EndIfMissingException : SyntaxErrorException
    {
        public EndIfMissingException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("If/ElseIf/Else not matched with EndIf"); 
        }
  
    }
}
