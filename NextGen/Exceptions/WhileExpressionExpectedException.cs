using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class WhileExpressionExpectedException : SyntaxErrorException
    {
        public WhileExpressionExpectedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Expression expected"); 
        }
  
    }
}
