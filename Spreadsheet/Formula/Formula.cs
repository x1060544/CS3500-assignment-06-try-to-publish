// Skeleton written by Joe Zachary for CS 3500, January 2017

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

        private String formula;
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

            int count = 0;
            int[] tokenTyp = new int[5];
            int lastToken = 0;
            int preToken = 0;

            foreach (String token in GetTokens(formula))
                {
                    if (isvarPattern (token))
                    {
                        double digit;
                        bool isDigit = double.TryParse (token, out digit);
                        
                        if (! isDigit)
                        {
                            count++;
                            preToken = lastToken;
                            lastToken = 2;
                            isLegal(count, lastToken, preToken, tokenTyp, false);
                        }

                        else
                        {
                            count++;
                            preToken = lastToken;
                            lastToken = 1;
                            isLegal(count, lastToken, preToken, tokenTyp, false);
                        }

                    }

                    else if (isopPattern (token))
                    {
                        count++;
                        preToken = lastToken;
                        lastToken = 3;
                        isLegal(count, lastToken, preToken, tokenTyp, false);                        
                    }

                    else if (islpPattern (token))
                    {
                        count++;
                        preToken = lastToken;
                        lastToken = 4;
                        isLegal(count, lastToken, preToken, tokenTyp, false); 
                    }

                    else if (isrpPattern (token))
                    {
                        count++;
                        preToken = lastToken;
                        lastToken = 5;
                        isLegal(count, lastToken, preToken, tokenTyp, false); 
                    }

                    else
                    {
                        throw new FormulaFormatException (message: "The symbol is undefined");
                    }                

                }
            isLegal(count, lastToken, preToken, tokenTyp, true);

            this.formula = formula;
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
            Stack<string> operators = new Stack<string>();
            Stack<string> values = new Stack<string>();
            double result = 0;

            foreach (String token in GetTokens(formula))
            {
                double digit;
                bool isDigit = double.TryParse(token, out digit);
                if (isDigit)
                {
                    String oper = operators.Pop();
                    if (Regex.IsMatch(oper, @"\*"))
                    {
                        String topValue = values.Pop();
                        double topNumber;
                        if (double.TryParse(topValue, result: out topNumber))
                        {
                            result = digit * topNumber;
                            values.Push(result.ToString());
                        }
                        else
                        {
                            result = (digit * lookup(topValue));
                            values.Push(result.ToString());
                        }
                    }

                    else if (Regex.IsMatch(oper, @"\/"))
                    {
                        String topValue = values.Pop();
                        double topNumber;
                        if (double.TryParse(topValue, result: out topNumber))
                        {
                            result = (topNumber / digit);
                            values.Push(result.ToString());
                        }
                        else
                        {
                            result = (lookup(topValue) / digit);
                            values.Push(result.ToString());
                        }
                    }
                    else
                    {
                        operators.Push(oper);
                        values.Push(token);
                    }

                }

                if (!isDigit)
                {
                    String oper = operators.Pop();
                    if (Regex.IsMatch(oper, @"\*"))
                    {
                        String topValue = values.Pop();
                        double topnumber;
                        if (double.TryParse(topValue, result: out topnumber))
                        {
                            result = (lookup(token) * topnumber);
                            values.Push(result.ToString());
                        }
                        else
                        {
                            result = (lookup(token) * lookup(topValue));
                            values.Push(result.ToString());
                        }
                    }
                    else if (Regex.IsMatch(oper, @"\/"))
                    {
                        String topValue = values.Pop();
                        double topNumber;
                        if (double.TryParse(topValue, result: out topNumber))
                        {
                            result = (topNumber / lookup(token));
                            values.Push(result.ToString());
                        }
                        else
                        {
                            result = (lookup(topValue) / lookup(token));
                            values.Push(result.ToString());
                        }
                    }
                    else
                    {
                        operators.Push(oper);
                        values.Push(token);
                    }
                }

                else if (isopPattern(token))
                {
                    if (Regex.IsMatch(token, @"\+") || Regex.IsMatch(token, @"\-"))
                    {
                        if (Regex.IsMatch(operators.Peek(), @"\+"))
                        {
                            String firstValue = values.Pop();
                            String secondValue = values.Pop();
                            double firstNumber;
                            double secondNumber;
                            if (double.TryParse(firstValue, result: out firstNumber))
                            {
                                if (double.TryParse(secondValue, result: out secondNumber))
                                {
                                    result = firstNumber + secondNumber;
                                    values.Push(result.ToString());
                                }

                                else
                                {
                                    result = firstNumber + lookup(secondValue);
                                    values.Push(result.ToString());
                                }
                            }
                            else
                            {
                                if (double.TryParse(secondValue, result: out secondNumber))
                                {
                                    result = lookup(firstValue) + secondNumber;
                                    values.Push(result.ToString());
                                }

                                else
                                {
                                    result = lookup(firstValue) + lookup(secondValue);
                                    values.Push(result.ToString());
                                }
                            }
                        }

                        else
                        {
                            String firstValue = values.Pop();
                            String secondValue = values.Pop();
                            double firstNumber;
                            double secondNumber;
                            if (double.TryParse(firstValue, result: out firstNumber))
                            {
                                if (double.TryParse(secondValue, result: out secondNumber))
                                {
                                    result = secondNumber - firstNumber;
                                    values.Push(result.ToString());
                                }

                                else
                                {
                                    result = lookup(secondValue) - firstNumber;
                                    values.Push(result.ToString());
                                }
                            }
                            else
                            {
                                if (double.TryParse(secondValue, result: out secondNumber))
                                {
                                    result = lookup(secondValue) - firstNumber;
                                    values.Push(result.ToString());
                                }

                                else
                                {
                                    result = lookup(secondValue) + lookup(firstValue);
                                    values.Push(result.ToString());
                                }
                            }
                        }

                        operators.Push(token);
                    }

                    else
                    {
                        operators.Push(token);
                    }
                }

                else if (islpPattern(token))
                {
                    operators.Push(token);
                }

                else if (isrpPattern(token))
                {
                    if (Regex.IsMatch(token, @"\+"))
                    {
                        String firstValue = values.Pop();
                        String secondValue = values.Pop();
                        double firstNumber;
                        double secondNumber;
                        if (double.TryParse(firstValue, result: out firstNumber))
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = firstNumber + secondNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = firstNumber + lookup(secondValue);
                                values.Push(result.ToString());
                            }
                        }
                        else
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = lookup(firstValue) + secondNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = lookup(firstValue) + lookup(secondValue);
                                values.Push(result.ToString());
                            }
                        }
                    }

                    else if (Regex.IsMatch(token, @"\-"))
                    {
                        String firstValue = values.Pop();
                        String secondValue = values.Pop();
                        double firstNumber;
                        double secondNumber;
                        if (double.TryParse(firstValue, result: out firstNumber))
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = secondNumber - firstNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = lookup(secondValue) - firstNumber;
                                values.Push(result.ToString());
                            }
                        }
                        else
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = lookup(secondValue) - firstNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = lookup(secondValue) + lookup(firstValue);
                                values.Push(result.ToString());
                            }
                        }
                    }

                    operators.Pop();

                    if (Regex.IsMatch(token, @"\*"))
                    {
                        String firstValue = values.Pop();
                        String secondValue = values.Pop();
                        double firstNumber;
                        double secondNumber;
                        if (double.TryParse(firstValue, result: out firstNumber))
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = firstNumber * secondNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = firstNumber * lookup(secondValue);
                                values.Push(result.ToString());
                            }
                        }
                        else
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = lookup(firstValue) * secondNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = lookup(firstValue) * lookup(secondValue);
                                values.Push(result.ToString());
                            }
                        }
                    }

                    else if (Regex.IsMatch(token, @"\/"))
                    {
                        String firstValue = values.Pop();
                        String secondValue = values.Pop();
                        double firstNumber;
                        double secondNumber;
                        if (double.TryParse(firstValue, result: out firstNumber))
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = secondNumber / firstNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = lookup(secondValue) / firstNumber;
                                values.Push(result.ToString());
                            }
                        }
                        else
                        {
                            if (double.TryParse(secondValue, result: out secondNumber))
                            {
                                result = lookup(secondValue) / firstNumber;
                                values.Push(result.ToString());
                            }

                            else
                            {
                                result = lookup(secondValue) / lookup(firstValue);
                                values.Push(result.ToString());
                            }
                        }
                    }
                }

                else
                {
                    throw new FormulaEvaluationException(message: "something wrong");
                }
            }

            if (operators.Count == 0)
            {
                double myResult;
                if (double.TryParse(values.Pop(), result: out myResult))
                {
                    return myResult;
                }

                else
                {
                    throw new FormulaEvaluationException(message: "Wrong output");
                }
            }

            else
            {
                if (Regex.IsMatch(operators.Peek(), @"\+"))
                {
                    String firstValue = values.Pop();
                    String secondValue = values.Pop();
                    double firstNumber;
                    double secondNumber;
                    if (double.TryParse(firstValue, result: out firstNumber))
                    {
                        if (double.TryParse(secondValue, result: out secondNumber))
                        {
                            result = firstNumber + secondNumber;
                            values.Push(result.ToString());
                        }

                        else
                        {
                            result = firstNumber + lookup(secondValue);
                            values.Push(result.ToString());
                        }
                    }
                    else
                    {
                        if (double.TryParse(secondValue, result: out secondNumber))
                        {
                            result = lookup(firstValue) + secondNumber;
                            values.Push(result.ToString());
                        }

                        else
                        {
                            result = lookup(firstValue) + lookup(secondValue);
                            values.Push(result.ToString());
                        }
                    }
                }

                else
                {
                    String firstValue = values.Pop();
                    String secondValue = values.Pop();
                    double firstNumber;
                    double secondNumber;
                    if (double.TryParse(firstValue, result: out firstNumber))
                    {
                        if (double.TryParse(secondValue, result: out secondNumber))
                        {
                            result = secondNumber - firstNumber;
                            values.Push(result.ToString());
                        }

                        else
                        {
                            result = lookup(secondValue) - firstNumber;
                            values.Push(result.ToString());
                        }
                    }
                    else
                    {
                        if (double.TryParse(secondValue, result: out secondNumber))
                        {
                            result = lookup(secondValue) - firstNumber;
                            values.Push(result.ToString());
                        }

                        else
                        {
                            result = lookup(secondValue) + lookup(firstValue);
                            values.Push(result.ToString());
                        }
                    }
                }

                double myResult;
                if (double.TryParse(values.Pop(), result: out myResult))
                {
                    return myResult;
                }

                else
                {
                    throw new FormulaEvaluationException(message: "Wrong output");
                }
            }
        }


        private bool isvarPattern(String token) => Regex.IsMatch(token, @"[a-zA-Z][0-9a-zA-Z]*");
        private bool islpPattern(String token) => Regex.IsMatch(token, @"\(");
        private bool isrpPattern(String token) => Regex.IsMatch(token, @"\)");
        private bool isopPattern(String token) => Regex.IsMatch(token, @"[\+\-*/]");
    
        private void isLegal (int count, int last, int pre, int[] tokens, bool over)
        {
            if (count == 1)
            {
                if (last == 3 || last == 5)
                    {throw new FormulaFormatException(message: "The first token of a formula must be a number, a variable, or an opening parenthesis");}
            }

            if (tokens[4] < tokens[5])
                {throw new FormulaFormatException(message: "closing parentheses seen so far be greater than the number of opening parentheses seen so far");}

            if (over)
            {
                if (last == 3 || last == 4)
                    {throw new FormulaFormatException(message: "No avaliable variables");}
            }

            if (last == 3 || last == 4)
            {
                if (!(pre == 1 || pre == 2 || pre == 4))
                    {throw new FormulaFormatException(message: "operator or  parenthesis is illegal");}
            }

            if (last == 1 || last == 2 || last == 5)
            {
                if (!(pre == 3 || pre == 5))
                    {throw new FormulaFormatException(message: "operator or  parenthesis is illegal");}
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
