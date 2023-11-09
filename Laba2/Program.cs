class Program
{
    static void Main()
    {
        string expression = Console.ReadLine();

        List<double> numbers = new List<double>();
        List<string> operations = new List<string>();

        string[] symbol = expression.Split(new char[] { '+', '-', '*', '/', '(', ')' },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (string a in symbol)
        {
            if (double.TryParse(a, out double number))
            {
                numbers.Add(number);
            }
        }

        foreach (char a in expression)
        {
            if (a == '+' || a == '-' || a == '*' || a == '/' || a == '(' || a == ')') operations.Add(a.ToString());
        }
        
        Console.WriteLine(string.Join(" ", numbers));
        Console.WriteLine(string.Join(" ", operations));

        for (int i = 0; i < operations.Count;)
        {
            if (operations[i] == "*")
            {
                numbers[i] *= numbers[i + 1];
                operations.RemoveAt(i);
                numbers.RemoveAt(i + 1);
            }

            else if (operations[i] == "/")
            {
                numbers[i] /= numbers[i + 1];
                operations.RemoveAt(i);
                numbers.RemoveAt(i + 1);
            }

            else i++;
        }

        for (int i = 0; i < operations.Count;)
        {
            if (operations[i] == "+")
            {
                numbers[i] += numbers[i + 1];
                operations.RemoveAt(i);
                numbers.RemoveAt(i + 1);
            }

            else if (operations[i] == "-")
            {
                numbers[i] -= numbers[i + 1];
                operations.RemoveAt(i);
                numbers.RemoveAt(i + 1);
            }

            else i++;
        }

        Console.WriteLine(string.Join(" ", numbers));
    }
}