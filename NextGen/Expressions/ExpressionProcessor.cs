using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Generator.Exceptions;

namespace Generator.Expressions
{
    public class ExpressionProcessor
    {
        private IExternalTokenResolver myTokenResolver;

		public ExpressionProcessor(IExternalTokenResolver tokenresolver)
		{
            myTokenResolver                         = tokenresolver;
		}

		public Var Evaluate(string expression)
		{
            System.Globalization.NumberFormatInfo 
										numberformat= new System.Globalization.NumberFormatInfo	();
			numberformat.NumberDecimalSeparator		= ".";
			
			int							i;

			mOperands								= new Stack();
			mOperators								= new Stack();

			// Simulate expression in brackets
			
			PushOperator(new OperatorInfo("(", -1, ""));
			expression								= expression + ").";

            bool                        bLastTokenWasOperand
                                                    = false;

			while (expression != ".")
			{
				// Allow for continue from multiple levels.
                bool                    bGotoNextPart
                                                    = false;

				// first eat all spaces
				if (Char.IsWhiteSpace(expression[0]))
				{
					do
					{
						expression					= expression.Remove(0, 1);
					} while (Char.IsWhiteSpace(expression[0]));
					continue;
				}
					
				// Look for an operator. If found process it...
				foreach (OperatorInfo op in OperatorInfo.cvOperators.Values)
				{
					if (expression.StartsWith(op.name) && (!op.IsText || !Char.IsLetter(expression[op.name.Length]) ))
					{
                        if (op.name == ")" || op.name == ",")
						{
							// Make sure any awaiting operations are processed first
							do
							{
								ProcessOperator();
							} while (mTopOperator.name != "(" && mTopOperator.name != ","); 
							// Now push the ) and process the () or function call.
							PushOperator(op);
							ProcessOperator();
						}
                        //else if (!bLastTokenWasOperand && !op.function)
                        //{
                        //    throw new OperandExpectedException(0, expression.Substring(0, expression.Length - 2));
                        //}
                        else if ((op.precedence > mTopOperator.precedence)
                            || (mTopOperator.name == "("))
                        {
                            // Next operator is higher in precedence. Process it first.
                            PushOperator(op);
                        }
                        else
                        {
                            do
                            {
                                ProcessOperator();
                            } while (op.precedence <= mTopOperator.precedence && mTopOperator.name != "(" && mTopOperator.name != ",");
                            PushOperator(op);
                        }
						bLastTokenWasOperand		= false;
						expression					= expression.Remove(0, op.name.Length);
						bGotoNextPart				= true;
						break;
					}
				}
				if (bGotoNextPart)
					continue;

                if (bLastTokenWasOperand)
                    throw new Exceptions.OperatorExpectedException(null, expression.Substring(0, expression.Length - 2));

				// Now check to see if first part of expression is a number
				if (Char.IsDigit(expression, 0))
				{
					DecimalVar			v			= new DecimalVar();
					i								= 0;
					// Eat the integer part of the number
					do
					{
						i++;
					} while (Char.IsDigit(expression, i));
					if (expression[i] == '.')
					{
						// It is a decimal
						do
						{
							i++;
						} while (Char.IsDigit(expression, i));
						v.v							= Decimal.Parse(expression.Substring(0,	i),System.Globalization.NumberStyles.Number, numberformat);
					}
					else
					{
						// It is an integer
						v.v							= Int32.Parse(expression.Substring(0, i));
					}
					v.empty							= false;
					mOperands.Push(v);
					expression						= expression.Remove(0, i);
                    bLastTokenWasOperand            = true;
					continue;
				}

				// Check if first part is string constant
				if (expression[0] == '\"')
				{
					i								= 1;
					StringVar			v			= new StringVar();
					while (i < expression.Length && expression[i] != '\"')
					{
						if (expression[i] == '\\')
						{
                            if ((i + 1) == expression.Length)
                                throw new StringTerminatorExpectedException(null, expression);
							switch (expression[i+1])
							{
								case '\"':
									v.v				+= "\"";
									break;
								case '\\':
									v.v				+= "\\";
									break;
								case 't':
									v.v				+= "\t";
									break;
								case 'n':
									v.v				+= "\n";
									break;
								default:
									v.v				+= expression[i+1].ToString();
									break;
							}
							// Shift additional position
							i++;
						}
						else
							v.v						+= expression[i].ToString();
						i++;
						v.empty						= false;
					} 
					if (i >= expression.Length)
                        throw new StringTerminatorExpectedException(null, expression.Substring(0, expression.Length - 2)); 

					mOperands.Push(v);
					expression						= expression.Remove(0, i+1);
                    bLastTokenWasOperand            = true;
                    continue;
				}

                // Eat all identifiers, separated by '.' and make it
                // a big token. Let the delegate decide which part it
                // can process.
				bool					bTokenEnded	= false;
				i									= 0;
                while (! bTokenEnded)
                {
                    if (! Char.IsLetter(expression, i))
                    {
						bTokenEnded					= true;
						break;
                    }
                    i++;

                    // identifier consists of letter + 0/more letter/digit/_
                    while (Char.IsLetterOrDigit(expression, i)
                        || expression[i] == '_')
                        i++;
                    if (expression[i] != '.')
                    {
						bTokenEnded					= true;
						break;
                    }
                    i++; // Read over the '.' and get next identifier
                }
                if (i > 0)
                {
					StringVar			sToken		= new StringVar(expression.Substring(0,	i));
					
					Var					r			= CheckConstantValue(sToken);
					if (r == null)      r           = myTokenResolver.TokenEvaluator(sToken); 
                    
                    if (r == null)
                    {
                        FunctionInfo    fi          = myTokenResolver.FunctionFinder(sToken.v);
                        if (fi != null)
                        {
                            OperatorInfo oi         = new OperatorInfo(fi.Name, 98, null, false, true);
                            PushOperator(oi);
                            expression              = expression.Remove(0, i);
                            bLastTokenWasOperand    = true;
                            continue;
                        }
                    }
                    else
                    {
                        // Store the result on the operand stack.
                        mOperands.Push(r);
                        // Remove the tokens we ate, and replace it with the 
                        // remainders of the tokenevaluation
						expression					= expression.Remove(0, i);
						expression					= sToken.v + expression;
						bGotoNextPart				= true;
                        bLastTokenWasOperand        = true;
                    }
                }

				if (bGotoNextPart)
					continue;

                throw new UnknownTokenException(null ,expression.Substring(0,i)); //this new StringVar(String.Format("Syntax error. '{0}' unexpected.", expression.Substring(0,i)));
				//throw new ApplicationException(String.Format("Syntax error. '{0}' unexpected.", expression.Substring(0,i)));
			}

            if (bLastTokenWasOperand)
                throw new OperatorExpectedException(null, expression.Substring(-1, expression.Length - 2));

			//ProcessOperator();
			if (mOperands.Count == 0)
				return null;
			else
				return (mOperands.Pop() as Var);
		}


		private Var CheckConstantValue(StringVar token)
		{
			if (String.Compare(token.v, "true", true) == 0)
			{
				token.v								= "";
				return new BooleanVar(true);
			}
			if (String.Compare(token.v, "false", true) == 0)
			{
				token.v								= "";
				return new BooleanVar(false);
			}
			return null;

		}

        public bool FindOperator(System.Reflection.MemberInfo m, object o)
		{
			OperatorInfo				op			= (o as	OperatorInfo);
			if (op.methodname == m.Name)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Process the top operator on the operator stack.
		/// Consume as many operands as needed from the operands stack.
		/// When top-operator is closing bracket, two options:
		///   a * ( b + c )
		///   In(a, b)
		/// </summary>
		private void ProcessOperator()
		{
			ArrayList					arglist		= new ArrayList();

			// Process closing bracket. Pop operators and process these
			// until an opening bracket is found on the stack.
			OperatorInfo				op			= PopOperator();
			if (op.name == ")")
			{
				op									= PopOperator();
				while (op.name != "(") 
				{
					System.Diagnostics.Debug.Assert(op.name == ",", "Er zou alleen een ',' op de stack mogen staan of een '(', en niet een '" + op.name + "'");
					arglist.Insert(0, mOperands.Pop());
					op								= PopOperator();
				} 

				if (arglist.Count > 0)
				{
					if (mTopOperator == null || ! mTopOperator.function)
						throw new ApplicationException(String.Format("Argument list found but not function call before '('"));
				}
				else
				{
					// No arglist found.
					// If function, pop first argument, else it was () combination. Return
					if (mTopOperator == null || ! mTopOperator.function)
						return;
				}
				// It is a function call
				// Pop first argument
				arglist.Insert(0, mOperands.Pop());
				op									= PopOperator();
			}
			else if (op.name == "," || op.name == "(")
			{
				PushOperator(op);	// do not process yet!
				return;
			}
			else
			{
				// It must be an operator. It can be unary or not, so get at least one operand.
				arglist.Insert(0, mOperands.Pop());
				if (! op.unary)
					arglist.Insert(0, mOperands.Pop());
			}

			Var							o1			= (arglist[0] as Var);
            if (op.methodname == null && op.function)
            {
                // Process function
                // Haal eerst de functieinformatie op.
                Var result = myTokenResolver.FunctionEvaluator(op.name, arglist);
                mOperands.Push(result);
            }
            else
            {
                System.Type t1 = o1.GetType();
                MemberInfo[] ts = t1.FindMembers(
                    System.Reflection.MemberTypes.Method,
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
                    new System.Reflection.MemberFilter(FindOperator),
                    op);

                if (ts.GetLength(0) == 0)
                    throw new ApplicationException(String.Format("Operation '{0}' not defined for '{1}'", op.name, o1.GetType().Name));

                // Find the method taking the right type of parameter
                MethodInfo methodFound = null;
                foreach (MethodInfo method in ts)
                {
                    ParameterInfo[] parms = method.GetParameters();

                    if (parms.GetLength(0) == arglist.Count)
                    {
                        methodFound = method;
                        for (int pi = 0; pi < arglist.Count; pi++)
                            if (parms[pi].ParameterType != arglist[pi].GetType())
                            {
                                methodFound = null;
                                break;
                            }
                        if (methodFound != null)
                            break;
                    }

                }
                if (methodFound == null)
                {
                    string msg = String.Format("Operation '{0}' not defined for ", op.name);
                    foreach (Var v in arglist)
                        msg += "'" + v.GetType().Name + "',";
                    throw new ApplicationException(msg);
                    //				if (op.unary)
                    //					throw new ApplicationException(String.Format("Operation '{0}' not defined for '{1}'", op.name, o1.GetType().Name));
                    //				else
                    //					throw new ApplicationException(String.Format("Operation '{0}' not defined for '{1}' and '{2}'", op.name, o1.GetType().Name, o2.GetType().Name));
                }

                // Method found. Apply it
                object[] args = arglist.ToArray();
                //			if (op.unary)
                //				args = new object[] {o1};
                //			else
                //				args = new object[] {o1, o2};

                if (methodFound.ReturnType == typeof(void))
                {
                    methodFound.Invoke(null, args);
                }
                else
                {
                    object result = methodFound.Invoke(null, args);
                    mOperands.Push(result);
                }
            }
			return;
		}

		private void PushOperator(OperatorInfo op)
		{
			mOperators.Push(op);
			mTopOperator							= op;
		}
		private OperatorInfo PopOperator()
		{
			OperatorInfo				r			= null;
			if (mOperators.Count > 0)
				r									= (OperatorInfo) mOperators.Pop();

			if (mOperators.Count > 0)
				mTopOperator						= (OperatorInfo) mOperators.Peek();
			else
				mTopOperator						= null;
			return r;
		}
		private	Stack					mOperands;
		private	Stack					mOperators;
		private	OperatorInfo			mTopOperator; // Top operator on the stack,	if stack  empty: null

    }
}
