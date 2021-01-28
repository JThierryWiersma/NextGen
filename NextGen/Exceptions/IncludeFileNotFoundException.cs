using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class IncludeFileNotFoundException : SyntaxErrorException
    {
        public IncludeFileNotFoundException(SourceCodeContext line, String path)
            : base(line, path)
        {
            _msg = String.Format("Include file not found: {0}", path);
        }
    }
}
