using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Generator.Expressions
{
    public class ConceptVar : Var
    {
        public XmlNode typedef;
        public XmlNode tvalue;

        public override string ToString()
        { return "iets"; }
        public ConceptVar(XmlNode aTypedef)
        {
            typedef = aTypedef;
            empty = true;
        }
        public ConceptVar(ConceptVar s)
        {
            name = s.name;
            tvalue = s.tvalue;
            empty = s.empty;
        }
        public override void Clear()
        {
            tvalue = null;
            base.Clear();
        }
        public override string TypeName()
        {
            return typedef.ParentNode.Attributes["name"].Value;    
        }

        public override bool Equals(object o)
        { 
            return (o is ConceptVar && ((o as ConceptVar).tvalue == tvalue)); 
        }
        public override int GetHashCode()
        { 
            return tvalue.GetHashCode(); 
        }
        public static BooleanVar operator ==(ConceptVar a, ConceptVar b)
        { 
            return new BooleanVar(a.tvalue == b.tvalue); 
        }
        public static BooleanVar operator !=(ConceptVar a, ConceptVar b)
        { 
            return new BooleanVar(a.tvalue != b.tvalue); 
        }
        public static void /*ConceptVar*/ op_Assign(ConceptVar a, ConceptVar b)
        {
            a.typedef = b.typedef;
            a.tvalue = b.tvalue;
            a.empty = b.empty;
            return;
        }
        public static void /*ConceptVar*/ op_Assign(ConceptVar a, StringVar b)
        {
            a.tvalue = null;
            a.empty = true;
            return;
        }
        public static BooleanVar op_Empty(ConceptVar a)
        {
            return new BooleanVar(a.tvalue == null);
        }
        public override Var CreateClone()
        {
            return new ConceptVar(this);
        }
    }
}
