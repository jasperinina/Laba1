using System.Globalization;
using System.Text;

namespace ExpressionCalculatorWPF
{
    public abstract class Token
    {
        public override string ToString() => GetType().Name;
    }
    public class Number : Token
    {
        public double Value { get; }
        public Number(double value) => Value = value;
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }
    public class Operation : Token, IComparable<Operation>
    {
        public string Symbol { get; }
        public int Priority => Symbol switch
        {
            "^" => 3,
            "*" or "/" or "rt" => 2,
            "+" or "-" => 1,
            "log" or "sqrt" or "sin" or "cos" or "tg" or "ctg" => 4,
            "(" => 0,
            _ => throw new ArgumentException($"Неизвестный оператор {Symbol}")
        };
        public bool IsRightAssociative => Symbol == "^";
        public Operation(string symbol) => Symbol = symbol;

        public static bool operator ==(Operation op1, Operation op2) => ReferenceEquals(op1, null) ? ReferenceEquals(op2, null) : op1.Equals(op2);
        public static bool operator !=(Operation op1, Operation op2) => !(op1 == op2);
        public static bool operator >(Operation op1, Operation op2) => op1.Priority > op2.Priority;
        public static bool operator <(Operation op1, Operation op2) => op1.Priority < op2.Priority;
        public static bool operator >=(Operation op1, Operation op2) => op1.Priority >= op2.Priority;
        public static bool operator <=(Operation op1, Operation op2) => op1.Priority <= op2.Priority;
        public int CompareTo(Operation other) => other == null ? 1 : Priority.CompareTo(other.Priority) * (IsRightAssociative ? -1 : 1);
        public override bool Equals(object obj) => obj is Operation operation && Symbol == operation.Symbol;
        public override int GetHashCode() => Symbol.GetHashCode();
        public override string ToString() => Symbol;
    }
    public class Variable : Token
    {
        public char Name { get; }

        public Variable(char name) => Name = name;

        public override string ToString() => Name.ToString();
    }

    public static class Utilities
    {
        public static List<Token> ReversePolishNotation(string expression)
        {
            List<Token> finalRPN = new List<Token>();
            Stack<Operation> oper = new Stack<Operation>();
            StringBuilder currentNum = new StringBuilder();
            
            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];
                if (char.IsDigit(ch) || ch == '.')
                {
                    currentNum.Clear();
                    do
                    {
                        currentNum.Append(ch);
                        i++;
                        if (i < expression.Length) ch = expression[i];
                    }
                    while (i < expression.Length && (char.IsDigit(ch) || ch == '.'));
                    finalRPN.Add(new Number(double.Parse(currentNum.ToString(), CultureInfo.InvariantCulture)));
                    i--;
                }
                else if (char.IsLetter(ch)) 
                {
                    if (ch == 'x') 
                    {
                        finalRPN.Add(new Variable('x'));
                    }
                    else 
                    {
                        if (TryGetOperator(expression, i, out string opSymbol, out int opLength))
                        {
                            ProcessOperator(opSymbol, oper, finalRPN);
                            i += opLength - 1;
                        }
                    }
                }
                else if (ch == '(')
                {
                    oper.Push(new Operation("("));
                }
                else if (ch == ')')
                {
                    while (oper.Count > 0 && oper.Peek().Symbol != "(")
                    {
                        finalRPN.Add(oper.Pop());
                    }
                    
                    if (oper.Count == 0) throw new InvalidOperationException("Несовпадающие круглые скобки.");
                    oper.Pop();

                    if (oper.Count > 0 && IsFunction(oper.Peek().Symbol))
                    {
                        finalRPN.Add(oper.Pop());
                    }
                }
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/') 
                {
                    ProcessOperator(ch.ToString(), oper, finalRPN);
                }
                else if (TryGetOperator(expression, i, out string opSymbol, out int opLength))
                {
                    ProcessOperator(opSymbol, oper, finalRPN);
                    i += opLength - 1;
                }
                else if (!char.IsWhiteSpace(ch))
                {
                    throw new InvalidOperationException($"Неизвестный оператор или символ: {ch}.");
                }
            }

            while (oper.Count > 0)
            {
                if (oper.Peek().Symbol == "(") throw new InvalidOperationException("Несовпадающие круглые скобки.");
                finalRPN.Add(oper.Pop());
            }

            return finalRPN;
        }

        private static void ProcessOperator(string symbol, Stack<Operation> oper, List<Token> finalRPN)
        {
            Operation newOp;
            
            if (IsFunction(symbol))
            {
                newOp = new Operation(symbol);
                oper.Push(newOp);
            }
            else
            {
                newOp = new Operation(symbol);
                
                while (oper.Count > 0 && ShouldPop(oper.Peek(), newOp))
                {
                    finalRPN.Add(oper.Pop());
                }
                
                if (oper.Count == 0 || oper.Peek().Symbol == "(" || newOp.Priority > oper.Peek().Priority)
                {
                    oper.Push(newOp);
                }
                else
                {
                    finalRPN.Add(oper.Pop());
                    oper.Push(newOp);
                }
            }
        }

        private static bool ShouldPop(Operation topOfStack, Operation currentOp)
        {
            if (currentOp.Symbol == "(") return false;
            if (topOfStack.Symbol == "(") return false;
            if (currentOp.Priority == topOfStack.Priority) return !currentOp.IsRightAssociative;
            if (currentOp.Priority == 4) return false;
            
            return topOfStack.Priority > currentOp.Priority;
        }

        private static bool IsFunction(string symbol)
        {
            return new HashSet<string> {"log", "sqrt", "sin", "cos", "tg", "ctg", "rt"}.Contains(symbol);
        }
        
        private static bool TryGetOperator(string expression, int index, out string opSymbol, out int opLength)
        {
            string[] allOperators = { "+", "-", "*", "/", "^", "log", "sqrt", "sin", "cos", "tg", "ctg", "rt" };
            opSymbol = null;
            opLength = 0;
            
            if (char.IsLetter(expression[index]))
            {
                StringBuilder functionName = new StringBuilder();
                functionName.Append(expression[index]);
                index++;
                
                while (index < expression.Length && char.IsLetter(expression[index]))
                {
                    functionName.Append(expression[index]);
                    index++;
                }
                
                opSymbol = functionName.ToString();
                opLength = opSymbol.Length;
                
                return true;
            }
            
            foreach (string op in allOperators)
            {
                if (expression.Substring(index).StartsWith(op))
                {
                    opSymbol = op;
                    opLength = op.Length;
                    return true;
                }
            }
            
            return false;
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
                    if (!ProcessOperation(op, values))
                    {
                        throw new InvalidOperationException($"Не удалось обработать операцию: {op.Symbol}.");
                    }
                }
            }

            return values.Count > 0 ? values.Pop() : null;
        }
        
        private static bool ProcessOperation(Operation op, Stack<double> values)
        {
            double num2, num1;
            
            int argsNeeded = op.Symbol switch
            {
                "+" or "-" or "*" or "/" or "^" or "rt" or "log"=> 2,
                "sqrt" or "sin" or "cos" or "tg" or "ctg" => 1,
                _ => throw new InvalidOperationException($"Неизвестная операция {op.Symbol}.")
            };

            if (values.Count < argsNeeded) return false;

            num2 = argsNeeded > 1 ? values.Pop() : 0;
            num1 = values.Pop();
            
            switch (op.Symbol)
            {
                case "+": values.Push(num1 + num2); break;
                case "-": values.Push(num1 - num2); break;
                case "*": values.Push(num1 * num2); break;
                case "/": values.Push(num1 / num2); break;
                case "^": values.Push(Math.Pow(num1, num2)); break;
                case "rt": values.Push(Math.Pow(num1, 1.0 / num2)); break;
                case "log":
                    if (num1 <= 0 || num2 <= 0 || num2 == 1) return false;
                    values.Push(Math.Log(num2, num1));
                    break;
                case "sqrt":
                    if (num1 < 0) return false;
                    values.Push(Math.Sqrt(num1));
                    break;
                case "sin": values.Push(Math.Sin(num1)); break;
                case "cos": values.Push(Math.Cos(num1)); break;
                case "tg": values.Push(Math.Tan(num1)); break;
                case "ctg": values.Push(1.0 / Math.Tan(num1)); break;
                default: return false;
            }
            
            return true;
        }
    }   
}
