using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.Write("Введите математическое выражение: ");
        string expression = Console.ReadLine();

        List<double> numbers = new List<double>(ListsNumAndOper(expression).Item2);
        List<string> operations = new List<string>(ListsNumAndOper(expression).Item1);
        
        Console.Write("Цифры: ");
        Console.WriteLine(string.Join(" ", numbers));
        Console.Write("Операции: ");
        Console.WriteLine(string.Join(" ", operations));
        Console.Write("Результат: ");
        Console.WriteLine(string.Join(" ",(PriorityAndCalculator(operations, numbers))));
    }
    
    static (List<string>, List<double>) ListsNumAndOper(string expression)
    {
        List<double> numbers = new List<double>();
        List<string> operations = new List<string>();
        
        var currentNumber = "";
        for (int i = 0; i < expression.Length; i++)
        {
            char a = expression[i];

            if (char.IsDigit(a) || a == '.') currentNumber += a;
            else
            {
                if (!string.IsNullOrWhiteSpace(currentNumber))
                {
                    numbers.Add(double.Parse(currentNumber, CultureInfo.InvariantCulture));
                    currentNumber = "";
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(currentNumber))
        {
            numbers.Add(double.Parse(currentNumber, CultureInfo.InvariantCulture));
        }

        foreach (char a in expression)
        {
            if (a == '+' || a == '-' || a == '*' || a == '/' || a == '(' || a == ')') operations.Add(a.ToString());
        }

        return (operations, numbers);
    }
    
    static List<double> PriorityAndCalculator(List<string> operations, List<double> numbers)
    {
        int currentPriority = 1;
        var operationsList = new List<PriorityOfOperations>();
        int maxPriority = 0;

        for (int i = 0; i < operations.Count; i++)
        {
            if (operations[i] == "(") currentPriority += 2;

            else if (operations[i] == ")") currentPriority -= 2;

            else
            {
                int priority = (operations[i] == "*" || operations[i] == "/") ? currentPriority + 1 : currentPriority;

                if (maxPriority < priority) maxPriority = priority;

                operationsList.Add(new PriorityOfOperations { Operation = operations[i], Priority = priority });
            }
        }
        
        for (int i = maxPriority; i > 0; i--)
        {
            for (int j = 0; j < operationsList.Count;)
            {
                if (i == operationsList[j].Priority)
                {
                    numbers[j] = Calculator(numbers[j], numbers[j + 1], operationsList[j].Operation);
                    operationsList.RemoveAt(j);
                    numbers.RemoveAt(j + 1);
                }
                else j++;
            }
        }
        return (numbers);
    }
    
    static double Calculator(double num1, double num2, string oper)
    {
        switch (oper)
        {
            case "+": return num1 + num2;
            case "-": return num1 - num2;
            case "*": return num1 * num2;
            case "/": return num1 / num2;
        }

        return 0;
    }
}

struct PriorityOfOperations
{
    public string Operation;
    public int Priority;
}