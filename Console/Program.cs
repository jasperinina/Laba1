using RPN.Logic;
using System;
using System.Collections.Generic;
using Logic;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите математическое выражение:");
            var input = Console.ReadLine();

            var calculator = new Calculator();
            var tokens = calculator.Parse(input);

            Console.WriteLine("\nОбратная польская запись: ");
            var postfix = calculator.ReversePolishNotation(tokens);
            foreach (var token in postfix)
            {
                if (token is Number number)
                {
                    Console.Write($"{number.Value} ");
                }
                else if (token is Operation operation)
                {
                    Console.Write($"{operation.Name} ");
                }
                else if (token is Variable variable)
                {
                    Console.Write($"{variable.Symbol} ");
                }
                else if (token is Comma)
                {
                    Console.Write(", ");
                }
            }

            var variableValues = new Dictionary<string, double>();

            foreach (var token in tokens)
            {
                if (token is Variable variable)
                {
                    Console.WriteLine($"\nВведите значение для переменной {variable.Symbol}:");
                    if (double.TryParse(Console.ReadLine(), out var value))
                    {
                        variableValues[variable.Symbol] = value;
                    }
                    else
                    {
                        Console.WriteLine($"Некорректное значение для переменной {variable.Symbol}.");
                        return;
                    }
                }
            }

            try
            {
                double result = calculator.CalculatingAnExpression(postfix, variableValues);
                Console.WriteLine("\n\nРезультат вычисления: ");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nОшибка: " + ex.Message);
            }
        }
    }
}
