using System.Globalization;

class Program
{
    static void Main()
    {
        Console.Write("Введите математическое выражение: ");
        string expression = Console.ReadLine();
        expression = expression.Replace(",", ".");

        Console.Write("Цифры: ");
        Console.WriteLine(string.Join(" ", ListsNumAndOper(expression).Item2));
        Console.Write("Операции: ");
        Console.WriteLine(string.Join(" ", ListsNumAndOper(expression).Item1));
        Console.Write("ОПЗ: ");
        Console.WriteLine(string.Join(" ", ReversePolishNotation(expression)));

    }

    //Метод для перевода в ОПЗ
    static List<string> ReversePolishNotation(string expression)
    {
        List<string> finalRPN = new List<string>();
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
                
                finalRPN.Add(currentNum);
                i--;
            }

            else if (expression[i] == '(') oper.Push(expression[i]);

            else if (expression[i] == ')')
            {
                while (oper.Count > 0 && oper.Peek() != '(') //пока стек не пустой и элемент стека не равен ( мы добавляем в финальное выражение все элементы до (
                    finalRPN.Add(oper.Pop().ToString());

                if (oper.Count > 0) oper.Pop(); //удаляет (
            }

            else if ((expression[i] == '+') || (expression[i] == '-') || (expression[i] == '*') || (expression[i] == '/'))
            {
                while (oper.Count != 0 && Priority(oper.Peek()) >= Priority(expression[i]))
                    finalRPN.Add(oper.Pop().ToString());
                oper.Push(expression[i]);
                /*
                Если оператор на вершине стека имеет приоритет, равный или больший,
                чем текущий оператор, это означает, что его нужно обработать
                (вытолкнуть из стека и добавить в выходной список) перед тем, как добавить текущий оператор
                */
            }
        }

        while (oper.Count != 0)
            finalRPN.Add(oper.Pop().ToString());

        return finalRPN;
    }

    static int Priority(char op)
    {
        if (op == '*' || op == '/') return 2;
        if (op == '+' || op == '-') return 1;
        return 0;
    }

    //Метод для вывода списка чисел и операций
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
}