using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class LoopNotMatchedException : SyntaxErrorException
    {
        public LoopNotMatchedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Loop not matched with Do or While"); 
        }
  
    }
}
