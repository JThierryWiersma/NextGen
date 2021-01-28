using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Generator.Expressions
{
    public abstract class Var
    {
        protected Var()
        {
        }
        public string name;
        public bool empty;
        public virtual void Clear()
        {
            empty                                   = true;
        }
        public abstract String TypeName(); 
        public static Var ConstructVar(string typename)
        {
            switch(typename)
            {
                case "String":
                    return new StringVar();
                case "Decimal":
                    return new DecimalVar();
                case "Boolean":
                    return new BooleanVar();
                default:
                    XmlNode     ttype               = null;
                    foreach (XmlNode n in Generator.Utility.TemplateCache.Instance().GetTypesList("TypeDefs"))
                    {
                        if (n.Attributes["name"].Value == typename)
                        {
                            ttype                   = n;
                            break;
                        }
                    }
                    if (ttype == null)
                        throw new ApplicationException(String.Format("Variable declaration: concept type not found: ('{0}')", typename));

                    return new ConceptVar(ttype);
            }
        }
        public abstract Var CreateClone();

    }
}
