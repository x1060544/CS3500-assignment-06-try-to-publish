// Skeleton written by Joe Zachary for CS 3500, January 2017

// The rest written by Yuntong Lu (u1060544), January 25 2017

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {

        private String _formula;
        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            // some references track the format
            int count = 0;
            // [0]: letter, [1]: double, [2]: operator, [3]: (, [4]: )
            int[] tokenTyp = new int[5];
            int lastToken = 0;
            int preToken = 0;

            // working throw each tokens, determain the token type
            foreach (String token in GetTokens(formula))
                {

                // if the token is variables,
                // determain if it is the double
                // update the references values
                    if (isvarPattern (token))
                    {
                        double digit;
                        bool isDigit = double.TryParse (token, out digit);
                        
                        if (! isDigit)
                        {
                            count++;
                            preToken = lastToken;
                            lastToken = 1;
                            tokenTyp[0]++;
                            isLegal(count, lastToken, preToken, tokenTyp, false);
                        }

                        else
                        {
                            count++;
                            preToken = lastToken;
                            lastToken = 2;
                            tokenTyp[1]++;
                            isLegal(count, lastToken, preToken, tokenTyp, false);
                        }

                    }
                // for all the rest of the if
                // once determained the token type
                // update the reference and check if the fomula is legal
                    else if (isopPattern (token))
                    {
                        count++;
                        preToken = lastToken;
                        lastToken = 3;
                        tokenTyp[2]++;
                        isLegal(count, lastToken, preToken, tokenTyp, false);                        
                    }
                
                    else if (islpPattern (token))
                    {
                        count++;
                        preToken = lastToken;
                        lastToken = 4;
                        tokenTyp[3]++;
                        isLegal(count, lastToken, preToken, tokenTyp, false); 
                    }

                    else if (isrpPattern (token))
                    {
                        count++;
                        preToken = lastToken;
                        lastToken = 5;
                        tokenTyp[4]++;
                        isLegal(count, lastToken, preToken, tokenTyp, false); 
                    }
                // if the token is none type, throw exception
                    else
                    {
                        throw new FormulaFormatException (message: token + "The symboal is undefinded");
                    }                
                }

            // check if the formula is legal after all the individual tokens are legal 
            isLegal(count, lastToken, preToken, tokenTyp, true);

            // get the formula
            _formula = formula;
        }
        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            // use two stacks to treck the variables and operators
            Stack<string> operators = new Stack<string>();
            Stack<double> values = new Stack<double>();
            double result = 0;

            // go through each token and determain the token type
            foreach (String token in GetTokens(_formula))
            {
                // if the token is variable, check if the token is double
                // if it is the double, avoid to use loopup delegate
                // if it is the letter, use delegate to convert the letter to double
                if (isvarPattern(token))
                {
                    // in those statements, once the divide operator is applied
                    // check the denomuroter, make sure it isn't zero
                    double digit;
                    bool isDigit = double.TryParse(token, out digit);
                    if (isDigit)
                    {
                        // alway check the operators before the next step
                        if (operators.Count > 0)
                        {

                            String oper = operators.Pop();
                            if (Regex.IsMatch(oper, @"\*"))
                            {

                                double topNumber = values.Pop();

                                result = digit * topNumber;
                                values.Push(result);

                            }

                            else if (Regex.IsMatch(oper, @"\/"))
                            {

                                double topNumber = values.Pop();

                                if (digit <= 1e-8)
                                {
                                    throw new FormulaEvaluationException(message: "don't divide zero!");
                                }
                                result = (topNumber / digit);
                                values.Push(result);
              
                            }
                            else
                            {
                                operators.Push(oper);
                                values.Push(digit);
                            }
                        }
                        // just push the value
                        else
                        {
                            values.Push(digit);
                        }
                    }

                    if (!isDigit)
                    {
                        if (operators.Count > 0)
                        {
                            String oper = operators.Pop();
                            if (Regex.IsMatch(oper, @"\*"))
                            {
                                try
                                {
                                    double myNumber = lookup(token);
                                }
                                catch
                                {
                                    throw new FormulaEvaluationException(message: "The lettler value is undefined");
                                }
                                result = (lookup(token) * values.Pop());
                                values.Push(result);     
                            }
                            else if (Regex.IsMatch(oper, @"\/"))
                            {
                                try
                                {
                                    double myNumber = lookup(token);
                                }
                                catch
                                {
                                    throw new FormulaEvaluationException(message: "The lettler value is undefined");
                                }

                                if (lookup(token) <= 1e-8)
                                {
                                 
                                    throw new FormulaEvaluationException(message: "don't divide zero!");
                                }

                                result = (values.Pop() / lookup(token));
                                values.Push(result);
                            }
                            else
                            {
                                try
                                {
                                    double myNumber = lookup(token);
                                }
                                catch
                                {
                                    throw new FormulaEvaluationException(message: "The lettler value is undefined");
                                }
                                operators.Push(oper);
                                values.Push(lookup(token));
                            }
                        }

                        else
                        {
                            try
                            {
                                double myNumber = lookup(token);
                            }
                            catch
                            {
                                throw new FormulaEvaluationException(message: "The lettler value is undefined");
                            }
                            values.Push(lookup(token));
                        }
                    }
                }

                // for + and - operator pattern, check the top of the operator stack
                // then apply them. for the / and * operator, just push them
                else if (isopPattern(token))
                {
                    if (Regex.IsMatch(token, @"\+") || Regex.IsMatch(token, @"\-"))
                    {
                        if (values.Count > 1 && operators.Count == 0)
                        {
                            throw new FormulaEvaluationException(message: "operator stack is empty");
                        }
                        if (values.Count > 1 && Regex.IsMatch(operators.Peek(), @"\+"))
                        {

                            double firstNumber = values.Pop();
                            double secondNumber = values.Pop();
                            result = firstNumber + secondNumber;
                            operators.Pop();
                            values.Push(result);
                            operators.Push(token);
                        }

                        else if (values.Count > 1 && Regex.IsMatch(operators.Peek(), @"\-"))
                        {

                            double firstNumber = values.Pop();
                            double secondNumber = values.Pop();

                            result = secondNumber - firstNumber;
                            operators.Pop();
                            values.Push(result);
                            operators.Push(token);

                        }

                        else
                        {
                            operators.Push(token);
                        }
                    }

                    else
                    {
                        operators.Push(token);
                    }
                }

                // push the left (
                else if (islpPattern(token))
                {
                    operators.Push(token);
                }

                // for the ), check the operator, once the operator is = or -, apply them
                // then, pop out the extra (, then apply the rest operators
                else if (isrpPattern(token))
                {
                    // use the peek(), this can prevent the pop without attentions
                    if (Regex.IsMatch(operators.Peek(), @"\+"))
                    {
                        double firstNumber = values.Pop();
                        double secondNumber = values.Pop();

                        result = firstNumber + secondNumber;
                        values.Push(result);
                        operators.Pop();
                    }

                    else if (Regex.IsMatch(operators.Peek(), @"\-"))
                    {
                        double firstNumber = values.Pop();
                        double secondNumber = values.Pop();

                        result = secondNumber - firstNumber;
                        values.Push(result);

                        operators.Pop();
                    }
                    // alway pop here
                    operators.Pop();
                    // check the operator stack, make sure it isn't empty
                    if (operators.Count > 0)
                    {
                        if (Regex.IsMatch(operators.Peek(), @"\*"))
                        {
                            double firstNumber = values.Pop();
                            double secondNumber = values.Pop();

                            result = firstNumber * secondNumber;
                            values.Push(result);

                            operators.Pop();
                        }

                        else if (Regex.IsMatch(operators.Peek(), @"\/"))
                        {
                            double firstNumber = values.Pop();
                            double secondNumber = values.Pop();

                            if(firstNumber <= 1e-8)
                            {
                                throw new FormulaEvaluationException(message: "don't divide zero!");
                            }
                            result = secondNumber / firstNumber;
                            values.Push(result);

                            operators.Pop();

                        }
 
                    }
                }

            }

            // push the result
            if (operators.Count == 0)
            {
                return values.Pop();
            }

            // apply the last opeartor and values, then push the result
            else
            {
                if (Regex.IsMatch(operators.Peek(), @"\+"))
                {
                    double firstNumber = values.Pop();
                    double secondNumber = values.Pop();

                    result = firstNumber + secondNumber;
                    values.Push(result);
                }

                else
                {
                    double firstNumber = values.Pop();
                    double secondNumber = values.Pop();

                    result = secondNumber - firstNumber;
                    values.Push(result);
                }

                return values.Pop();

            }
        }

        /**
         * For those four bool method, checking the token type
         * */
        private bool isvarPattern(String token) => (Regex.IsMatch(token, @"^\d$") || Regex.IsMatch(token, @"[a-zA-Z][0-9a-zA-Z]*")
            || double.TryParse(token, out double number));
        private bool islpPattern(String token) => Regex.IsMatch(token, @"\(");
        private bool isrpPattern(String token) => Regex.IsMatch(token, @"\)");
        private bool isopPattern(String token) => Regex.IsMatch(token, @"[\+\-*/]");

        /// <summary>
        /// this method is use to determain wheather the token and the formula is legal
        /// There can be no invalid tokens.
        /// There must be at least one token.
        /// When reading tokens from left to right, at no point should the
        /// number of closing parentheses seen so far be greater than the number of opening parentheses seen so far.
        /// The total number of opening parentheses must equal the total number of closing parentheses.
        /// The first token of a formula must be a number, a variable, or an opening parenthesis.
        /// The last token of a formula must be a number, a variable, or a closing parenthesis.
        /// Any token that immediately follows an opening parenthesis or an operator must be either 
        /// a number, a variable, or an opening parenthesis.
        /// Any token that immediately follows a number, a variable, or a closing parenthesis must be
        /// either an operator or a closing parenthesis.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="last"></param>
        /// <param name="pre"></param>
        /// <param name="tokens"></param>
        /// <param name="over"></param>
        private void isLegal (int count, int last, int pre, int[] tokens, bool over)
        {
            if (count == 1)
            {
                if (last == 3 || last == 5)
                    {throw new FormulaFormatException(message: "The first token of a formula must be a number, a variable, or an opening parenthesis");}
            }

            if (tokens[3] < tokens[4])
                {throw new FormulaFormatException(message: "closing parentheses seen so far be greater than the number of opening parentheses seen so far");}

            if (over)
            {
                if (tokens[3] != tokens[4] || last == 3 || last == 4 || (tokens[0]+ tokens[1]+ tokens[2]+ tokens[3]+ tokens[4] < 1))
                    {throw new FormulaFormatException(message: "No avaliable variables");}
            }

            if (pre == 3 || pre == 4)
            {
                if (last == 3 || last == 5)
                    {throw new FormulaFormatException(message: "operator or  parenthesis is illegal");}
            }

            if (pre == 1 || pre == 2 || pre == 5)
            {
                if (last == 1 || last == 2 || last == 4)
                    {throw new FormulaFormatException(message:"operator or  parenthesis is illegal");}
            }
        }
        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens.
            // NOTE:  These patterns are designed to be used to create a pattern to split a string into tokens.
            // For example, the opPattern will match any string that contains an operator symbol, such as
            // "abc+def".  If you want to use one of these patterns to match an entire string (e.g., make it so
            // the opPattern will match "+" but not "abc+def", you need to add ^ to the beginning of the pattern
            // and $ to the end (e.g., opPattern would need to be @"^[\+\-*/]$".)
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";

            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.  This pattern is useful for 
            // splitting a string into tokens.
            String splittingPattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, splittingPattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }


}
