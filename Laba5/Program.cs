using System;
using ExpressionCalculatorWPF;

class Program
{
    static void Main()
    {
        Console.Write("Введите математическое выражение с переменной x: ");
        string expression = Console.ReadLine().Replace(",", ".");
        
        Console.Write("Введите значение переменной x: ");
        string inputValue = Console.ReadLine().Replace(",", ".");
        double xValue;
        while (!double.TryParse(inputValue, out xValue))
        {
            Console.Write("Некорректный ввод, введите еще раз: ");
            inputValue = Console.ReadLine().Replace(",", ".");
        }

        Console.Write("ОПЗ: ");
        var rpn = Utilities.ReversePolishNotation(expression);
        foreach (var token in rpn)
        {
            if (token is Number number)
            {
                Console.Write($"{number.Value} ");
            }
            else if (token is Operation operation)
            {
                Console.Write($"{operation} "); 
            }
            else if (token is Variable variable)
            {
                Console.Write($"{variable.Name} ");
            }
        }
        Console.WriteLine();

        Console.Write("Результат: ");
        double? result = Utilities.CalculatingValue(rpn, xValue); 
        if (result.HasValue)
        {
            double actualResult = result.Value; 
            Console.WriteLine(actualResult.ToString("0.####"));
        }
        else
        {
            Console.WriteLine("Результат не может быть вычислен.");
        }
    }
}