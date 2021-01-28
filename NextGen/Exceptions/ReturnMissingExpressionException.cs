using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ReturnMissingExpressionException : SyntaxErrorException
    {
        public ReturnMissingExpressionException(SourceCodeContext linenr)
            : base(linenr)
        {
            _msg = String.Format("Return requires an expression with the function result");
        }
    }
}
