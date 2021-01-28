using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class TooManyFunctionParametersException : SyntaxErrorException
    {
        private string _functionname;
        private int _expected; 
        private int _supplied;

        public TooManyFunctionParametersException(SourceCodeContext linenr, string functionname, int expected, int supplied)
            : base(linenr)
        {
            _functionname = functionname;
            _expected = expected;
            _supplied = supplied;
            _msg = String.Format("Function '{0}' has {1} parameters, but {2} parameters were supplied", _functionname, _expected, _supplied);
        }

    }
}
