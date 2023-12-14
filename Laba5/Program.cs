using System;
using System.Collections.Generic;
using System.Globalization;

abstract class Token
{
    
}

class Number : Token
{
    public double Value { get; private set; }

    public Number(double value)
    {
        Value = value;
    }
}

class Operation : Token
{
    public char Symbol { get; private set; }
    public int Priority 
    { 
        get 
        {
            return Symbol == '*' || Symbol == '/' ? 2 : 1;
        }
    }

    public Operation(char symbol)
    {
        Symbol = symbol;
    }

    public static bool operator ==(Operation op1, Operation op2)
    {
        return op1.Symbol == op2.Symbol;
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
    
}

class Parenthesis : Token
{
    public char Symbol { get; private set; }

    public Parenthesis(char symbol)
    {
        Symbol = symbol;
    }
}

class Program
{
    static void Main()
    {
        Console.Write("Введите математическое выражение: ");
        string expression = Console.ReadLine();
        expression = expression.Replace(",", ".");

        Console.Write("ОПЗ: ");
        List<Token> rpn = ReversePolishNotation(expression);
        foreach (var token in rpn)
        {
            if (token is Number number)
                Console.Write($"{number.Value} ");
            else if (token is Operation operation)
                Console.Write($"{operation.Symbol} ");
        }
        Console.WriteLine();

        Console.Write("Результат: ");
        Console.WriteLine(CalculatingValue(rpn));
    }

    static List<Token> ReversePolishNotation(string expression)
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
                    currentNum += expression[i];
                    i++;
                }

                finalRPN.Add(new Number(double.Parse(currentNum, CultureInfo.InvariantCulture)));
                i--;
            }
            else if (expression[i] == '(')
            {
                oper.Push(new Operation(expression[i]));
            }
            else if (expression[i] == ')')
            {
                while (oper.Count > 0 && oper.Peek().Symbol != '(')
                    finalRPN.Add(oper.Pop());

                if (oper.Count > 0) oper.Pop();
            }
            else if ((expression[i] == '+') || (expression[i] == '-') || (expression[i] == '*') || (expression[i] == '/'))
            {
                Operation currentOp = new Operation(expression[i]);
                while (oper.Count != 0 && (oper.Peek() > currentOp || oper.Peek() == currentOp))
                    finalRPN.Add(oper.Pop());
                oper.Push(currentOp);

            }
        }

        while (oper.Count != 0)
            finalRPN.Add(oper.Pop());

        return finalRPN;
    }

    static double CalculatingValue(List<Token> finalRPN)
    {
        Stack<double> values = new Stack<double>();

        foreach (var token in finalRPN)
        {
            if (token is Number num)
            {
                values.Push(num.Value);
            }
            else if (token is Operation op)
            {
                double num2 = values.Pop();
                double num1 = values.Pop();
                switch (op.Symbol)
                {
                    case '+': values.Push(num1 + num2); break;
                    case '-': values.Push(num1 - num2); break;
                    case '*': values.Push(num1 * num2); break;
                    case '/': values.Push(num1 / num2); break;
                }
            }
        }

        return values.Pop();
    }
}
