using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpressionCalculator
{
    public abstract class Token
    {
    }

    public class Number : Token
    {
        public double Value { get; private set; }

        public Number(double value)
        {
            Value = value;
        }
    }

    public class Operation : Token
    {
        public char Symbol { get; private set; }
        public int Priority 
        { 
            get 
            {
                switch (Symbol)
                {
                    case '*':
                    case '/':
                        return 2;
                    case '+':
                    case '-':
                        return 1;
                    default:
                        throw new ArgumentException($"Unsupported operator {Symbol}");
                }
            }
        }

        public Operation(char symbol)
        {
            Symbol = symbol;
        }

        public static bool operator ==(Operation op1, Operation op2)
        {
            return op1?.Symbol == op2?.Symbol;
        }

        public static bool operator !=(Operation op1, Operation op2)
        {
            return !(op1 == op2);
        }

        public static bool operator >(Operation op1, Operation op2)
        {
            return op1.Priority > op2.Priority;
        }

        public static bool operator <(Operation op1, Operation op2)
        {
            return op1.Priority < op2.Priority;
        }

        public static bool operator >=(Operation op1, Operation op2)
        {
            return op1.Priority >= op2.Priority;
        }

        public static bool operator <=(Operation op1, Operation op2)
        {
            return op1.Priority <= op2.Priority;
        }

        public override bool Equals(object obj)
        {
            return obj is Operation operation && Symbol == operation.Symbol;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }
    }

    public class Variable : Token
    {
        public char Name { get; private set; }

        public Variable(char name)
        {
            Name = name;
        }
    }
    
    public class Parenthesis : Token
    {
        public char Symbol { get; private set; }

        public Parenthesis(char symbol)
        {
            Symbol = symbol;
        }
    }

    public static class Utilities
    {
        public static List<Token> ReversePolishNotation(string expression)
        {
            List<Token> finalRPN = new List<Token>();
            Stack<Operation> oper = new Stack<Operation>();

            for (int i = 0; i < expression.Length; i++)
            {
                if (char.IsDigit(expression[i]) || expression[i] == '.')
                {
                    string currentNum = "";
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        currentNum += expression[i++];
                    }
                    finalRPN.Add(new Number(double.Parse(currentNum, CultureInfo.InvariantCulture)));
                    i--;
                }
                else if (expression[i] == 'x')
                {
                    finalRPN.Add(new Variable('x'));
                }
                else if ("+-*/()".Contains(expression[i].ToString()))
                {
                    ProcessOperator(expression[i], oper, finalRPN);
                }
            }

            while (oper.Count > 0)
            {
                finalRPN.Add(oper.Pop());
            }

            return finalRPN;
        }

        private static void ProcessOperator(char symbol, Stack<Operation> oper, List<Token> finalRPN)
        {
            if (symbol == '(')
            {
                oper.Push(new Operation(symbol));
            }
            else if (symbol == ')')
            {
                while (oper.Count > 0 && oper.Peek().Symbol != '(')
                {
                    finalRPN.Add(oper.Pop());
                }
                oper.Pop(); // Pop the '('
            }
            else // Handling +, -, * or /
            {
                Operation currentOp = new Operation(symbol);
                while (oper.Count != 0 && (oper.Peek() >= currentOp))
                {
                    finalRPN.Add(oper.Pop());
                }
                oper.Push(currentOp);
            }
        }

        public static double CalculatingValue(List<Token> finalRPN, double xValue)
        {
            Stack<double> values = new Stack<double>();

            foreach (var token in finalRPN)
            {
                switch (token)
                {
                    case Number num:
                        values.Push(num.Value);
                        break;
                    case Variable _:
                        values.Push(xValue);
                        break;
                    case Operation op:
                        double num2 = values.Pop();
                        double num1 = values.Pop();

                        switch (op.Symbol)
                        {
                            case '+': values.Push(num1 + num2); break;
                            case '-': values.Push(num1 - num2); break;
                            case '*': values.Push(num1 * num2); break;
                            case '/': values.Push(num1 / num2); break;
                        }
                        break;
                }
            }
            return values.Pop();
        }
    }

}
