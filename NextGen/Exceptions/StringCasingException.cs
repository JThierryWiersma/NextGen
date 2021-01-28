using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class StringCasingException : SyntaxErrorException
    {
        public StringCasingException(string casing)
            : base(null, casing)
        {
            _msg =  String.Format("String casing error, operand '{0}' is not recognized", casing);
        }
    }
}
