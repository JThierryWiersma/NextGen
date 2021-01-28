using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class EmptyValueNotSuitableForOperationException : SyntaxErrorException
    {
        public EmptyValueNotSuitableForOperationException(string name, string fnorop)
            : base(null)
        {
            _msg = String.Format("Empty value for variable '{0}' not applicable for {1}", name, fnorop);
        }
    }
}
