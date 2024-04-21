using ExpressionCalculator;

class Program
{
    static void Main()
    {
        Console.Write("Введите математическое выражение: ");
        string expression = Console.ReadLine().Replace(",", ".");

        Console.Write("ОПЗ: ");
        var rpn = Utilities.ReversePolishNotation(expression);
        foreach (var token in rpn)
        {
            if (token is Number number)
                Console.Write($"{number.Value} ");
            else if (token is Operation operation)
                Console.Write($"{operation.Symbol} ");
        }
        Console.WriteLine();

        Console.Write("Результат: ");
        double result = Utilities.CalculatingValue(rpn);
        Console.WriteLine(result);
    }
}