using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class ReturnValueMissingException : SyntaxErrorException
    {
        public ReturnValueMissingException(SourceCodeContext linenr, string functionname)
            : base(linenr)
        {
            _msg = String.Format("No function result returned for function {0}", functionname);
        }
    }
}
