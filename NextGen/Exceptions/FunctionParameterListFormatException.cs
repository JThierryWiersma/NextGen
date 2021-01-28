using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class FunctionParameterListFormatException : SyntaxErrorException
    {
        public FunctionParameterListFormatException(SourceCodeContext linenr, string textinerror)
            : base(linenr, textinerror)
        {
            _msg = String.Format("Function parameterlist error in '{0}'", textinerror);
        }
    }
}
