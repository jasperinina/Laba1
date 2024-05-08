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
            Console.Write("Некорректный ввод! Пожалуйста, введите числовое значение для x: ");
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
                Console.Write($"{operation.Symbol} ");
            }
            else if (token is Variable variable)
            {
                Console.Write($"{variable.Name} ");
            }
        }
        Console.WriteLine();

        Console.Write("Результат: ");
        double result = Utilities.CalculatingValue(rpn, xValue); // Передаем rpn и значение x
        Console.WriteLine(result.ToString("0.####")); // Форматируем вывод, чтобы исключить излишнее количество знаков после запятой
    }
}