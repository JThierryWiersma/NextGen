using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class EndFunctionMissingException : SyntaxErrorException
    {
        public EndFunctionMissingException(SourceCodeContext linenr, string functionname)
            : base(linenr, functionname)
        {
            _msg = String.Format("EndFunction missing in Function {0}", functionname);
        }
    }
}
