using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class IncludeFilesCanOnlyContainFunctionsException : SyntaxErrorException
    {
        public IncludeFilesCanOnlyContainFunctionsException(SourceCodeContext line)
            : base(line)
        {
            _msg = "Include files can only contain functions, comments and empty lines";
        }
    }
}
