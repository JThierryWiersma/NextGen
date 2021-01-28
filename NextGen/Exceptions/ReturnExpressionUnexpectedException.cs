using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ReturnExpressionUnexpectedException : SyntaxErrorException
    {
        public ReturnExpressionUnexpectedException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Return for a Void function does not allow an expression");
        }
    }
}
