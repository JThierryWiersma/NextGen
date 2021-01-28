using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.Exceptions
{
    class FieldNotFoundException : SyntaxErrorException
    {
        private string _concept;
        private string _notfoundfield;

        public FieldNotFoundException(string msg, string concept, string notfoundfield) : base(null, notfoundfield)
        {
            _concept = concept;
            _notfoundfield = notfoundfield;
            _msg = String.Format("Field '{0}' is unknown in {1}", notfoundfield, concept); 
        }
    }

}
