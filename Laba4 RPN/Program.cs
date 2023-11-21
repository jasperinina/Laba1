using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.Write("Введите математическое выражение: ");
        string expression = Console.ReadLine();
        expression = expression.Replace(",", ".");

        Console.Write("ОПЗ: ");
        Console.WriteLine(string.Join(" ", ReversePolishNotation(expression)));
        
        Console.Write("Результат: ");
        Console.WriteLine(string.Join(" ", CalculatingValue(ReversePolishNotation(expression))));
    }

    static List<object> ReversePolishNotation(string expression)
    {
        List<object> finalRPN = new List<object>();
        Stack<char> oper = new Stack<char>();

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

                finalRPN.Add(double.Parse(currentNum, CultureInfo.InvariantCulture));
                i--;
            }
            else if (expression[i] == '(')
            {
                oper.Push(expression[i]);
            }
            else if (expression[i] == ')')
            {
                while (oper.Count > 0 && oper.Peek() != '(')
                    finalRPN.Add(oper.Pop());

                if (oper.Count > 0) oper.Pop();
            }
            else if ((expression[i] == '+') || (expression[i] == '-') || (expression[i] == '*') || (expression[i] == '/'))
            {
                while (oper.Count != 0 && Priority(oper.Peek()) >= Priority(expression[i]))
                    finalRPN.Add(oper.Pop());
                oper.Push(expression[i]);
            }
        }

        while (oper.Count != 0)
            finalRPN.Add(oper.Pop());

        return finalRPN;
    }

    static int Priority(char op)
    {
        if (op == '*' || op == '/') return 2;
        if (op == '+' || op == '-') return 1;
        return 0;
    }
    
    static List<double> CalculatingValue(List<object> finalRPN)
    {
        for (int i = 0; i < finalRPN.Count; i++)
        {
            double num1;
            double num2;
            
            if (finalRPN[i] is char oper && (oper == '+' || oper == '-' || oper == '*' || oper == '/') && i - 1 >= 0 && i - 2 >= 0)
            {
                num1 = (double)finalRPN[i - 2];
                num2 = (double)finalRPN[i - 1];
                    
                finalRPN[i] = Calculator(num1, num2, oper);
                finalRPN.RemoveAt(i - 1);
                finalRPN.RemoveAt(i - 2);
                i -= 2;
            }
        }

        List<double> result = new List<double>();

        foreach (var item in finalRPN)
        {
            if (item is double num)
            {
                result.Add(num);
            }
        }
        return result;
    }

    static double Calculator(double num1, double num2, char oper)
    {
        switch (oper)
        {
            case '+': return num1 + num2;
            case '-': return num1 - num2;
            case '*': return num1 * num2;
            case '/': return num1 / num2;
        }

        return 0;
    }
}