using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpressionCalculatorWPF
{
    public abstract class Token
    {
        // Базовый класс для всех токенов
        public override string ToString()
        {
            return GetType().Name;
        }
    }


    public class Number : Token
    {
        public double Value { get; }

        public Number(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }


    public class Operation : Token, IComparable<Operation>
{
    public string Symbol { get; }

    public int Priority
    {
        get
        {
            return Symbol switch
            {
                "^" => 3,
                "*" or "/" or "rt" => 2,
                "+" or "-" => 1,
                "log" or "sqrt" or "sin" or "cos" or "tg" or "ctg" => 4,
                "(" => 0,
                _ => throw new ArgumentException($"Неизвестный оператор {Symbol}")
            };
        }
    }

    public bool IsRightAssociative => Symbol == "^";

    public Operation(string symbol)
    {
        Symbol = symbol;
    }

    public static bool operator ==(Operation op1, Operation op2)
    {
        if (ReferenceEquals(op1, null) || ReferenceEquals(op2, null))
            return ReferenceEquals(op1, op2);
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

    public static bool operator >=(Operation op1, Operation op2)
    {
        return op1.Priority >= op2.Priority;
    }

    public static bool operator <=(Operation op1, Operation op2)
    {
        return op1.Priority <= op2.Priority;
    }

    public int CompareTo(Operation other)
    {
        if (other == null) return 1;

        // Учитываем ассоциативность
        if (IsRightAssociative)
            return Priority < other.Priority ? 1 : -1;
        return Priority > other.Priority ? 1 : -1;
    }

    public override bool Equals(object obj)
    {
        return obj is Operation operation && Symbol == operation.Symbol;
    }

    public override int GetHashCode()
    {
        return Symbol.GetHashCode();
    }

    public override string ToString()
    {
        return Symbol;
    }
}

    public class Variable : Token
    {
        public char Name { get; }

        public Variable(char name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }


    public static class Utilities
    {
        //Преобразования математического выражения в обратную польскую запись
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
                i--; // Откат на последний символ числа
            }
            else if (expression[i] == 'x')
            {
                finalRPN.Add(new Variable('x'));
            }
            else if (expression[i] == '(')
            {
                oper.Push(new Operation("("));
            }
            else if (expression[i] == ')')
            {
                while (oper.Count > 0 && oper.Peek().Symbol != "(")
                {
                    finalRPN.Add(oper.Pop());
                }
                if (oper.Count == 0)
                {
                    throw new InvalidOperationException("Несоответствие скобок");
                }
                oper.Pop(); // Удалить "(" из стека

                // Если после закрытия скобок идет функция, добавляем ее в финальный список
                if (oper.Count > 0 && IsFunction(oper.Peek().Symbol))
                {
                    finalRPN.Add(oper.Pop());
                }
            }
            else if (TryGetOperator(expression, i, out string opSymbol, out int opLength))
            {
                ProcessOperator(opSymbol, oper, finalRPN);
                i += opLength - 1; // Увеличить индекс на длину оператора минус 1
            }
            else if (!char.IsWhiteSpace(expression[i])) // Игнорируем пробелы
            {
                throw new InvalidOperationException($"Неизвестный оператор или символ: {expression[i]}");
            }
        }

        while (oper.Count > 0)
        {
            if (oper.Peek().Symbol == "(")
            {
                throw new InvalidOperationException("Несоответствие скобок");
            }
            finalRPN.Add(oper.Pop());
        }

        return finalRPN;
    }
        
        //Обработка операторов
        private static bool TryGetOperator(string expression, int index, out string opSymbol, out int opLength)
        {
            opSymbol = expression[index].ToString();
            opLength = 1;

            switch (opSymbol)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "^":
                    return true;
                default:
                    string[] multiOperators = { "log", "sqrt", "rt", "sin", "cos", "tg", "ctg" };
                    foreach (string multiOp in multiOperators)
                    {
                        if (expression.Substring(index).StartsWith(multiOp))
                        {
                            opSymbol = multiOp;
                            opLength = multiOp.Length;
                            return true;
                        }
                    }
                    return false;
            }
        }

        private static bool IsFunction(string symbol)
        {
            string[] functions = { "log", "sqrt", "sin", "cos", "tg", "ctg", "rt" };
            return Array.Exists(functions, func => func == symbol);
        }

        private static void ProcessOperator(string symbol, Stack<Operation> oper, List<Token> finalRPN)
        {
            if (symbol == "(")
            {
                oper.Push(new Operation(symbol));
            }
            else
            {
                Operation currentOp = new Operation(symbol);
                while (oper.Count != 0 && oper.Peek() is Operation prevOp && (prevOp > currentOp || (prevOp == currentOp && !currentOp.IsRightAssociative)))
                {
                    finalRPN.Add(oper.Pop());
                }
                oper.Push(currentOp);
            }
        }
        
        public static double? CalculatingValue(List<Token> finalRPN, double xValue)
        {
            Stack<double> values = new Stack<double>();

            foreach (var token in finalRPN)
            {
                if (token is Number num)
                {
                    values.Push(num.Value);
                }
                else if (token is Variable)
                {
                    values.Push(xValue);
                }
                else if (token is Operation op)
                {
                    double num2, num1;

                    switch (op.Symbol)
                    {
                        case "+":
                        case "-":
                        case "*":
                        case "/":
                        case "^":
                            if (values.Count < 2)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop();
                            num1 = values.Pop();
                            break;
                        case "log":
                            if (values.Count < 2)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop(); // Основание логарифма
                            num1 = values.Pop(); // Число, для которого вычисляется логарифм
                            if (num1 <= 0 || num2 <= 0 || num2 == 1)
                            {
                                throw new InvalidOperationException("Invalid arguments for logarithm");
                            }
                            values.Push(Math.Log(num2, num1)); // Вычисляем log(num2, num1) вместо log(num1, num2)
                            continue;
                        case "sqrt":
                            if (values.Count < 1)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop();
                            num1 = 0; // Для однопараметрических функций
                            values.Push(Math.Sqrt(num2));
                            continue;
                        case "rt":
                            if (values.Count < 2)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop(); // Степень корня
                            num1 = values.Pop(); // Число, из которого извлекаем корень
                            if (num1 < 0 && num2 % 2 == 0) // Проверяем, что число неотрицательное для чётной степени
                            {
                                throw new InvalidOperationException($"Cannot calculate square root of a negative number");
                            }
                            values.Push(Math.Pow(num1, 1.0 / num2)); // Вычисляем корень степени a из числа b
                            continue;
                        case "sin":
                            if (values.Count < 1)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop();
                            num1 = 0; // Для однопараметрических функций
                            values.Push(Math.Sin(num2 * Math.PI / 180.0));
                            continue;
                        case "cos":
                            if (values.Count < 1)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop();
                            num1 = 0; // Для однопараметрических функций
                            values.Push(Math.Cos(num2 * Math.PI / 180.0));
                            continue;
                        case "tg":
                            if (values.Count < 1)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop();
                            num1 = 0; // Для однопараметрических функций
                            values.Push(Math.Tan(num2 * Math.PI / 180.0));
                            continue;
                        case "ctg":
                            if (values.Count < 1)
                            {
                                throw new InvalidOperationException($"Not enough arguments for operation {op.Symbol}");
                            }
                            num2 = values.Pop();
                            num1 = 0; // Для однопараметрических функций
                            values.Push(1.0 / Math.Tan(num2 * Math.PI / 180.0));
                            continue;
                        default:
                            throw new InvalidOperationException($"Unknown operation {op.Symbol}");
                    }

                    switch (op.Symbol)
                    {
                        case "+":
                            values.Push(num1 + num2);
                            break;
                        case "-":
                            values.Push(num1 - num2);
                            break;
                        case "*":
                            values.Push(num1 * num2);
                            break;
                        case "/":
                            values.Push(num1 / num2);
                            break;
                        case "^":
                            values.Push(Math.Pow(num1, num2));
                            break;
                        case "log":
                            if (num1 <= 0 || num2 <= 0 || num2 == 1)
                            {
                                throw new InvalidOperationException("Invalid arguments for logarithm");
                            }
                            values.Push(Math.Log(num1, num2));
                            break;
                    }
                }
            }

            return values.Count > 0 ? values.Pop() : (double?)null;
        }


    }
}