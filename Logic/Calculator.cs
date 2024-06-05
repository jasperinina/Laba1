using Logic;

namespace RPN.Logic
{
    public class Calculator
    {
        private readonly Dictionary<string, Operation> _operations;

        public Calculator()
        {
            _operations = LoadOperations();
        }

        private Dictionary<string, Operation> LoadOperations()
        {
            var operations = new Dictionary<string, Operation>();
            var operationTypes = typeof(Operation).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Operation)) && !t.IsAbstract);

            foreach (var type in operationTypes)
            {
                try
                {
                    var instance = (Operation)Activator.CreateInstance(type);
                    operations[instance.Name] = instance;
                }
                catch (MissingMethodException)
                {
                    if (type == typeof(UnaryOperation))
                    {
                        operations["-"] = new UnaryOperation("-", 3);
                    }
                }
            }

            return operations;
        }

        public List<Token> Parse(string expression)
        {
            var tokens = new List<Token>();
            var index = 0;

            while (index < expression.Length)
            {
                if (char.IsWhiteSpace(expression[index]))
                {
                    index++;
                    continue;
                }

                if (char.IsDigit(expression[index]) || expression[index] == '.')
                {
                    var number = ParseNumber(expression, ref index);
                    tokens.Add(new Number { Value = number });
                }
                else if (char.IsLetter(expression[index]))
                {
                    var name = ParseName(expression, ref index);
                    if (_operations.ContainsKey(name))
                    {
                        tokens.Add(_operations[name]);
                    }
                    else
                    {
                        tokens.Add(new Variable { Symbol = name });
                    }
                }
                else if (expression[index] == '(' || expression[index] == ')')
                {
                    tokens.Add(new Parenthesis { Value = expression[index] });
                    index++;
                }
                else if (expression[index] == ',')
                {
                    tokens.Add(new Comma());
                    index++;
                }
                else
                {
                    var op = expression[index].ToString();
                    if (_operations.ContainsKey(op))
                    {
                        tokens.Add(_operations[op]);
                        index++;
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown token: {expression[index]}");
                    }
                }
            }

            return tokens;
        }

        public List<Token> ReversePolishNotation(List<Token> infixTokens)
        {
            var output = new List<Token>();
            var operators = new Stack<Operation>();

            foreach (var token in infixTokens)
            {
                if (token is Number || token is Variable)
                {
                    output.Add(token);
                }
                else if (token is Parenthesis parenthesis)
                {
                    if (parenthesis.Value == '(')
                    {
                        operators.Push(new LeftParenthesisOperation());
                    }
                    else
                    {
                        while (operators.Count > 0 && !(operators.Peek() is LeftParenthesisOperation))
                        {
                            output.Add(operators.Pop());
                        }

                        if (operators.Count == 0)
                        {
                            throw new ArgumentException("Mismatched parentheses");
                        }


                        operators.Pop();
                    }
                }
                else if (token is Comma)
                {
                    while (operators.Count > 0 && !(operators.Peek() is LeftParenthesisOperation))
                    {
                        output.Add(operators.Pop());
                    }
                }
                else if (token is Operation op)
                {
                    while (operators.Count > 0 && operators.Peek().Priority >= op.Priority)
                    {
                        output.Add(operators.Pop());
                    }

                    operators.Push(op);
                }
            }

            while (operators.Count > 0)
            {
                var topOp = operators.Pop();
                if (topOp is LeftParenthesisOperation)
                {
                    throw new ArgumentException("Mismatched parentheses");
                }

                output.Add(topOp);
            }

            return output;
        }

        public double EvaluatePostfix(List<Token> postfixTokens, Dictionary<string, double> variableValues)
        {
            var stack = new Stack<double>();

            foreach (var token in postfixTokens)
            {
                if (token is Number number)
                {
                    stack.Push(number.Value);
                }
                else if (token is Variable variable)
                {
                    if (!variableValues.TryGetValue(variable.Symbol, out var value))
                    {
                        throw new ArgumentException($"Unknown variable: {variable.Symbol}");
                    }
                    stack.Push(value);
                }
                else if (token is Operation op)
                {
                    var args = new double[op.ArgsCount];
                    for (var i = op.ArgsCount - 1; i >= 0; i--)
                    {
                        args[i] = stack.Pop();
                    }

                    var result = op.Execute(args);
                    stack.Push(result);
                }
            }

            return stack.Pop();
        }

        private double ParseNumber(string expression, ref int index)
        {
            var startIndex = index;
            while (index < expression.Length && (char.IsDigit(expression[index]) || expression[index] == '.'))
            {
                index++;
            }

            var numberStr = expression.Substring(startIndex, index - startIndex);
            return double.Parse(numberStr);
        }

        private string ParseName(string expression, ref int index)
        {
            var startIndex = index;
            while (index < expression.Length && char.IsLetter(expression[index]))
            {
                index++;
            }

            return expression.Substring(startIndex, index - startIndex);
        }
    }
}
