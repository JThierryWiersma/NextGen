using System;
using System.Collections.Generic;
using System.Text;
using Generator.Exceptions;

namespace Generator.Expressions
{
    public class DecimalVar : Var
    {
        public decimal v;
        public override string ToString()
        {
            return (empty ? "" : v.ToString());
        }

        public override void Clear()
        {
            v = 0;
            base.Clear();
        }

        public DecimalVar()
        {
            v = 0;
            empty = true;
        }
        public DecimalVar(Decimal d)
        {
            v = d;
            empty = false;
        }
        public DecimalVar(string s)
        {
            if (s.Trim() == "")
            {
                v = 0;
                empty = true;
            }
            else
            {
                try
                {
                    v = Decimal.Parse(s);
                    empty = false;
                }
                catch
                {
                    v = 0;
                    empty = true;
                }
            }
        }
        public DecimalVar(DecimalVar d)
        {
            name = d.name;
            v = d.v;
            empty = d.empty;
        }
        public override string TypeName()
        {
            return "Decimal";
        }

        public override bool Equals(object o)
        { 
            return (o is DecimalVar && ((o as DecimalVar).v == v)); 
        }
        public override int GetHashCode()
        { 
            return v.GetHashCode(); 
        }
        public static DecimalVar operator +(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'+'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'+'-operator");

            return new DecimalVar(a.v + b.v);
        }
        public static DecimalVar operator -(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'-'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'-'-operator");

            return new DecimalVar(a.v - b.v);
        }
        public static DecimalVar operator ++(DecimalVar a)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'++'-operator");

            a.v++;
            a.empty = false;
            return a;
        }
        public static DecimalVar operator --(DecimalVar a)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'--'-operator");

            a.v--;
            a.empty = false;
            return a;
        }
        public static DecimalVar operator *(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'*'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'*'-operator");

            return new DecimalVar(a.v * b.v); 
        }
        public static DecimalVar operator /(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'/'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'/'-operator");

            return new DecimalVar(a.v / b.v); 
        }

        public static BooleanVar operator ==(DecimalVar a, DecimalVar b)
        {
            return new BooleanVar(a.empty == b.empty && a.v == b.v); 
        }
        
        public static BooleanVar operator !=(DecimalVar a, DecimalVar b)
        { 
            return new BooleanVar(a.v != b.v); 
        }
        public static BooleanVar operator <(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'<'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'<'-operator");

            return new BooleanVar(a.v < b.v); 
        }
        public static BooleanVar operator >(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'>'-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'>'-operator");

            return new BooleanVar(a.v > b.v); 
        }
        public static BooleanVar operator <=(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'<='-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'<='-operator");

            return new BooleanVar(a.v <= b.v); 
        }
        public static BooleanVar operator >=(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'>='-operator");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'>='-operator");

            return new BooleanVar(a.v >= b.v); 
        }
        public static void /*DecimalVar*/ op_AssignAddition(DecimalVar a, DecimalVar b)
        {
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'+='-operator");

            if (!b.empty)
            {
                a.v += b.v;
                a.empty = false;
            }
        }

        public static void /*DecimalVar*/ op_AssignSubtraction(DecimalVar a, DecimalVar b)
        {
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'-='-operator");

            if (!b.empty)
            {
                a.v -= b.v;
                a.empty = false;
            }
            //return a;
        }
        public static void /*DecimalVar*/ op_AssignMultiply(DecimalVar a, DecimalVar b)
        {
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'*='-operator");

            a.v *= b.v; 
            a.empty = false; 
            return; 
        }
        public static void /*DecimalVar*/ op_AssignDivision(DecimalVar a, DecimalVar b)
        {
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'/='-operator");

            a.v /= b.v; 
            a.empty = false; 
            return; 
        }

        public static void /*DecimalVar*/ op_Assign(DecimalVar a, DecimalVar b)
        { 
            a.v = b.v; 
            a.empty = b.empty; 
        }
 
        public static DecimalVar op_Min(DecimalVar a, DecimalVar b)
        {
            // Alles is kleiner dan empty, lijkt mij een handige feature
            if (a.empty)
                if (b.empty)
                    return new DecimalVar();
                else
                    return new DecimalVar(b);
            else
                if (b.empty)
                    return new DecimalVar(a);
                else
                    return new DecimalVar(a.v < b.v ? a.v : b.v);
        }
        public static DecimalVar op_Min(DecimalVar a, DecimalVar b, DecimalVar c)
        {
            return op_Min(a, op_Min(b, c));
        }
        public static DecimalVar op_Max(DecimalVar a, DecimalVar b)
        {
            // Alles is groter dan empty
            if (a.empty)
                if (b.empty)
                    return new DecimalVar();
                else
                    return new DecimalVar(b);
            else
                if (b.empty)
                    return new DecimalVar(a);
                else
                    return new DecimalVar(a.v > b.v ? a.v : b.v);
        }
        public static DecimalVar op_Max(DecimalVar a, DecimalVar b, DecimalVar c)
        {
            return op_Max(a, op_Max(b, c));
        }
        public static StringVar op_ToString(DecimalVar a)
        {
            if (a.empty)
                return new StringVar();
            else
                return new StringVar(a.v.ToString());
        }
        public static BooleanVar op_Empty(DecimalVar a)
        {
            return new BooleanVar(a.empty);
        }
        public static DecimalVar op_Round(DecimalVar a, DecimalVar b)
        {
            if (a.empty)
                throw new EmptyValueNotSuitableForOperationException(a.name, "'Round'-function");
            if (b.empty)
                throw new EmptyValueNotSuitableForOperationException(b.name, "'Round'-function");

            return new DecimalVar(Decimal.Round(a.v, (int)b.v));
        }
        public override Var CreateClone()
        {
            return new DecimalVar(this);
        }
    }
}
