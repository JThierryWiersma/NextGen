using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Globalization;
using Generator.Exceptions;

namespace Generator.Expressions
{
    public class ExpressionProcessorV2
    {
        private NumberFormatInfo        mNumberformat;

        private IExternalTokenResolver  myTokenResolver;

		public ExpressionProcessorV2(IExternalTokenResolver tokenresolver)
		{
            myTokenResolver                         = tokenresolver;
            mNumberformat                           = new NumberFormatInfo();
            mNumberformat.NumberDecimalSeparator    = ".";
        }

        public Var Evaluate(string expression)
        {
            Var result;
			expression								= expression + " ";

            result                                  = Evaluate(ref expression, false);
            return result;
        }

		private Var Evaluate(ref string expression, bool recursive)
		{
            Stack<Var>                  mOperands   = new Stack<Var>();
            Stack<OperatorInfo>         mOperators  = new Stack<OperatorInfo>();
			
            // Initialize bounds 
			mOperators.Push(new OperatorInfo("", -1, ""));
            ParsingState                state       = ParsingState.expectingOperand;

			while (true)
			{
                Token                   token       = GetNextToken(ref expression);

                switch (state)
                {
                    case ParsingState.expectingOperand:
                        //---------------------------------
                        switch (token.tokentype)
                        {
                            case TokenType.constantNumber:
                                mOperands.Push(new DecimalVar(token.t));
                                state               = ParsingState.expectingOperator;
                                continue;

                            case TokenType.constantString:
                                mOperands.Push(new StringVar(token.t));
                                state               = ParsingState.expectingOperator;
                                continue;

                            case TokenType.identifier:
                                // Controleer of het een bekende constante is
                                Var     r           = CheckConstantValue(token.t);
                                if (r != null)
                                {
                                    mOperands.Push(r);
                                    state           = ParsingState.expectingOperator;
                                    continue;
                                }

                                // Controleer of het een standaard functie is
                                if (OperatorInfo.cvOperators.Contains(token.t))
                                {
                                    OperatorInfo op = OperatorInfo.cvOperators[token.t] as OperatorInfo;
                                    System.Diagnostics.Debug.Assert(op.function);
                                    mOperators.Push(op.Kloon());
                                    state = ParsingState.expectingParameterList;
                                    continue;
                                }

                                // Controleer op een Variabele referentie
                                r                   = myTokenResolver.TokenEvaluator(token.t);
                                if (r != null)
                                {
                                    mOperands.Push(r);
                                    state           = ParsingState.expectingOperator;
                                    continue;
                                }

                                // Controleer op een functieaanroep
                                FunctionInfo fi     = myTokenResolver.FunctionFinder(token.t);
                                if (fi != null)
                                {
                                    OperatorInfo op = new OperatorInfo(fi.Name, 98, null, false, true);
                                    mOperators.Push(op);
                                    state           = ParsingState.expectingParameterList;
                                    continue;
                                }

                                // Anders kan het niks zijn.
                                throw new SyntaxErrorException(null, token.t);

                            case TokenType.openhaak:
                                mOperands.Push(Evaluate(ref expression, true));
                                token               = GetNextToken(ref expression);
                                if (token.tokentype != TokenType.sluithaak)
                                    throw new ParenthesisCloseExpectedException(null, token.t);

                                state               = ParsingState.expectingOperator;
                                continue;

                            default:
                                throw new SyntaxErrorException(null, token.t);
                        }

                    case ParsingState.expectingOperator:
                        //---------------------------------

                        switch (token.tokentype)
                        {
                            case TokenType.sluithaak:
                            case TokenType.komma:
                            case TokenType.sluitblokhaak:
                                // Dat kan als we recursief zijn aangeroepen 
                                if (recursive)
                                {
                                    // un-get de token in de expressie.
                                    expression = token.ToString() + expression;
                                    while (mOperators.Peek().name != "")
                                    {
                                        ProcessOperator(mOperators.Pop(), mOperands);
                                    }
                                    System.Diagnostics.Debug.Assert(mOperands.Count == 1);
                                    System.Diagnostics.Debug.Assert(mOperators.Count == 1);

                                    return (mOperands.Count > 0 ? mOperands.Pop() : null);
                                }
                                throw new UnknownTokenException(null, token.t);

                            case TokenType.none:
                                // einde van de expressie. We moeten alle operatoren van de stack
                                // uitvoeren en uiteindelijk op 1 operand uitkomen en die
                                // teruggeven.
                                while (mOperators.Peek().name != "")
                                {
                                    ProcessOperator(mOperators.Pop(), mOperands);
                                }
                                System.Diagnostics.Debug.Assert(mOperands.Count <= 1);
                                System.Diagnostics.Debug.Assert(mOperators.Count == 1);

                                return (mOperands.Count > 0 ? mOperands.Pop() : null);

                            case TokenType.other:
                                // Check ff dat het echt een operator is die we kennen.
                                if (! OperatorInfo.cvOperators.Contains(token.t))
                                    throw new OperatorExpectedException(null, token.t);

                                OperatorInfo op = OperatorInfo.cvOperators[token.t] as OperatorInfo;
                                System.Diagnostics.Debug.Assert(!op.function);

                                while (op.precedence <= mOperators.Peek().precedence)
                                {
                                    ProcessOperator(mOperators.Pop(), mOperands);
                                }
                                mOperators.Push(op.Kloon());
                                state           = ParsingState.expectingOperand;
                                continue;

                            default:
                                throw new OperatorExpectedException(null, token.t);
                        }

                    case ParsingState.expectingParameterList:
                        //---------------------------------
                        switch (token.tokentype)
                        {
                            case TokenType.openhaak:
                                // Verwerk zolang operands tot er geen komma meer is.
                                token = GetNextToken(ref expression);
                                if (token.tokentype == TokenType.sluithaak)
                                {
                                    // het moet een functie zonder parameters zijn
                                    ProcessOperator(mOperators.Pop(), mOperands);
                                    state = ParsingState.expectingOperator;
                                    continue;
                                }
                                // Oeps, het was zeker het begin van een expressie die een parameter oplevert.
                                // Zet de token er weer voor.
                                expression = token.ToString() + expression;
                                do
                                {
                                    mOperators.Peek().parameters.Add(Evaluate(ref expression, true));
                                    token = GetNextToken(ref expression);
                                } while (token.tokentype == TokenType.komma);
                                
                                // en check dat dat dan wel een ')' is.
                                if (token.tokentype != TokenType.sluithaak)
                                    throw new ParenthesisCloseExpectedException(null, token.t);

                                ProcessOperator(mOperators.Pop(), mOperands);
                                state               = ParsingState.expectingOperator;
                                continue;

                            default:
                                throw new ParenthesisOpenExpectedException(null, token.t);
                        }

                    case ParsingState.expectingParameter:
                        //---------------------------------
                        System.Diagnostics.Debug.Assert(false);
                        continue;

                    case ParsingState.expectingArrayList:
                        //---------------------------------
                        System.Diagnostics.Debug.Assert(false);
                        break;
                    case ParsingState.expectingArrayIndex:
                        //---------------------------------
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }

            }					
		}


		private Var CheckConstantValue(String token)
		{
			if (String.Compare(token, "true", true) == 0)
			{
				return new BooleanVar(true);
			}
			if (String.Compare(token, "false", true) == 0)
			{
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
        private void ProcessOperator(OperatorInfo op, Stack<Var> operands)
		{
			ArrayList					arglist		= new ArrayList();

            if (op.function)
            {
                foreach (Var v in op.parameters)
                    arglist.Add(v);
            }
			else
			{
				// It must be an operator. It can be unary or not, so get at least one operand.
                arglist.Insert(0, operands.Pop());
				if (! op.unary)
					arglist.Insert(0, operands.Pop());
			}

            if (op.methodname == null && op.function)
            {
                // Process function
                // Haal eerst de functieinformatie op.
                Var                     result      = myTokenResolver.FunctionEvaluator(op.name, arglist);
                System.Diagnostics.Debug.Assert(result != null);
                operands.Push(result);
            }
            else
            {
                Var                     o1          = (arglist[0] as Var);
                System.Type             t1          = o1.GetType();
                MemberInfo[]            ts          = t1.FindMembers(
                    System.Reflection.MemberTypes.Method,
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
                    new System.Reflection.MemberFilter(FindOperator),
                    op);

                if (ts.GetLength(0) == 0)
                    throw new OperationNotDefinedException(null, op.name, o1.TypeName(), arglist);

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
                    throw new OperationNotDefinedException(null, op.name, o1.TypeName(), arglist);
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
                    operands.Push(result as Var);
                }
            }
			return;
		}

        //private void PushOperator(OperatorInfo op)
        //{
        //    mOperators.Push(op);
        //    mTopOperator							= op;
        //}
        //private OperatorInfo PopOperator()
        //{
        //    OperatorInfo				r			= null;
        //    if (mOperators.Count > 0)
        //        r									= (OperatorInfo) mOperators.Pop();

        //    if (mOperators.Count > 0)
        //        mTopOperator						= (OperatorInfo) mOperators.Peek();
        //    else
        //        mTopOperator						= null;
        //    return r;
        //}

        /// <summary>
        /// Zorg ervoor dat de expression altijd eindigt met minstens 1 spatie
        /// dan hoeven we niet voortdurend op einde van de string te testen.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Token GetNextToken(ref String expression)
        {
            Token                   result          = new Token();
            int                     i;

            while (expression != "" && Char.IsWhiteSpace(expression[0]))
			{
                result.prefixspaces++;
				expression					        = expression.Remove(0, 1);
            }

            if (expression == "")
            {
                result.tokentype                    = TokenType.none;
                return result;
            }

    		// Now check to see if first part of expression is a number
			if (Char.IsDigit(expression, 0))
			{
				i								    = 0;
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
				}
				result.t 						    = expression.Substring(0, i);
                result.tokentype                    = TokenType.constantNumber;
				expression						    = expression.Remove(0, i);
                return result;
			}

			// Check if first part is string constant
			if (expression[0] == '\"')
			{
				i								    = 1;
                result.tokentype                    = TokenType.constantString;
				while (i < expression.Length && expression[i] != '\"')
				{
					if (expression[i] == '\\')
					{
                        if ((i + 1) == expression.Length)
                            throw new StringTerminatorExpectedException(null, expression);
						switch (expression[i+1])
						{
							case '\"':
								result.t		    += "\"";
								break;
							case '\\':
								result.t		    += "\\";
								break;
							case 't':
								result.t		    += "\t";
								break;
							case 'n':
								result.t		    += "\n";
								break;
							default:
								result.t		    += expression[i+1].ToString();
								break;
						}
						// Shift additional position
						i++;
					}
					else
						result.t				    += expression[i].ToString();
					i++;
				} 
				if (i >= expression.Length)
                    throw new StringTerminatorExpectedException(null, expression.Substring(0, expression.Length - 2)); 

				expression						    = expression.Remove(0, i+1);
                return result;
			}

            if (expression[0] == '(')
            {
                result.tokentype                    = TokenType.openhaak;
                result.t                            = "(";
                expression                          = expression.Substring(1);
                return result;
            }
            if (expression[0] == ')')
            {
                result.tokentype                    = TokenType.sluithaak;
                result.t                            = ")";
                expression                          = expression.Substring(1);
                return result;
            }
            if (expression[0] == '[')
            {
                result.tokentype                    = TokenType.openblokhaak;
                result.t                            = "[";
                expression                          = expression.Substring(1);
                return result;
            }
            if (expression[0] == ']')
            {
                result.tokentype                    = TokenType.sluitblokhaak;
                result.t                            = "]";
                expression                          = expression.Substring(1);
                return result;
            }
            if (expression[0] == ',')
            {
                result.tokentype                    = TokenType.komma;
                result.t                            = ",";
                expression                          = expression.Substring(1);
                return result;
            }

            if (! Char.IsLetter(expression, 0))
            {
                // nu moet het een operatorachtig ding zijn.
                // lees net zolang tot letter of cijfer of spatie of haak open/sluit
                i = 1;
                while (! Char.IsLetterOrDigit(expression ,i) && !" ()[]\"".Contains(expression[i].ToString()))
                    i++;
                result.t                            = expression.Substring(0, i);
                expression                          = expression.Substring(i);
                result.tokentype                    = TokenType.other;
                return result;
            }

            System.Diagnostics.Debug.Assert(Char.IsLetter(expression, 0));

            // Eat all identifiers, separated by '.' and make it
            // a big token. Let the delegate decide which part it
            // can process.
			bool					    bTokenEnded	= false;
			i									    = 0;
            while (!bTokenEnded && i < expression.Length)
            {
                if (! Char.IsLetter(expression, i))
                {
					bTokenEnded					    = true;
					break;
                }
                i++;

                // identifier consists of letter + 0/more letter/digit/_
                while (Char.IsLetterOrDigit(expression, i)
                    || expression[i] == '_')
                    i++;
                if (expression[i] != '.')
                {
					bTokenEnded					    = true;
					break;
                }
                i++; // Read over the '.' and get next identifier
            }
            result.t                                = expression.Substring(0, i);
            expression                              = expression.Substring(i);
            result.tokentype                        = TokenType.identifier;
            return result;    
        }
    }

    public enum TokenType
    {
        constantNumber,
        constantString,
        identifier,
        openblokhaak,
        sluitblokhaak,
        openhaak, 
        sluithaak, komma,
        other,
        none
    }

    public enum ParsingState
    {
        expectingOperand,
        expectingParameterList,
        expectingParameter,
        expectingOperator,
        expectingArrayList,
        expectingArrayIndex,
        error
    }
    public class Token
    {
        public string t;
        public int prefixspaces;
        public TokenType tokentype;
        public Token()
        { 
            prefixspaces = 0;
            t = "";
            tokentype = TokenType.none;
        }
        /// <summary>
        /// Return het token zoals het ook in de expressie gelezen was.(met elke whitespace => 1x' ') 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String result = "";
            for (int i = 0; i < prefixspaces; i++)
                result += " ";

            // Vergeet de quotes er niet weer omheen te zetten.
            if (tokentype == TokenType.constantString)
                result += "\"" + t + "\"";
            else
                result += t;

            return result;
        }
    }
}
