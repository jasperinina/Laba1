namespace Logic
{
    public enum Associativity
    {
        Left,
        Right
    }

    public abstract class Operation : Token
    {
        public abstract string Name { get; }
        public abstract int Priority { get; }
        public abstract Associativity Associativity { get; }
        public abstract bool IsFunction { get; }
        public abstract int ArgsCount { get; }
        public abstract double Execute(params double[] numbers);
    }

    public class LeftParenthesisOperation : Operation
    {
        public override string Name => "(";
        public override int Priority => int.MinValue;
        public override Associativity Associativity => Associativity.Left;
        public override bool IsFunction => false;
        public override int ArgsCount => 0;
        public override double Execute(params double[] numbers) => throw new NotImplementedException();
    }

    public class Comma : Token
    {
        public override string ToString() => ",";
    }

    public class UnaryOperation : Operation
    {
        private readonly string _name;
        private readonly int _priority;

        public UnaryOperation(string name, int priority)
        {
            _name = name;
            _priority = priority;
        }

        public UnaryOperation() : this("-", 3)
        {
        }

        public override string Name => _name;
        public override int Priority => _priority;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => false;
        public override int ArgsCount => 1;

        public override double Execute(params double[] numbers)
        {
            if (numbers.Length != 1) throw new ArgumentException("Unary operation requires exactly one argument.");
            return _name == "-" ? -numbers[0] : numbers[0];
        }
    }

    public class Addition : Operation
    {
        public override string Name => "+";
        public override int Priority => 1;
        public override Associativity Associativity => Associativity.Left;
        public override bool IsFunction => false;
        public override int ArgsCount => 2;
        public override double Execute(params double[] numbers)
        {
            return numbers[0] + numbers[1];
        }
    }

    public class Subtraction : Operation
    {
        public override string Name => "-";
        public override int Priority => 1;
        public override Associativity Associativity => Associativity.Left;
        public override bool IsFunction => false;
        public override int ArgsCount => 2;
        public override double Execute(params double[] numbers)
        {
            return numbers[0] - numbers[1];
        }
    }

    public class Multiply : Operation
    {
        public override string Name => "*";
        public override int Priority => 2;
        public override Associativity Associativity => Associativity.Left;
        public override bool IsFunction => false;
        public override int ArgsCount => 2;
        public override double Execute(params double[] numbers)
        {
            return numbers[0] * numbers[1];
        }
    }

    public class Divide : Operation
    {
        public override string Name => "/";
        public override int Priority => 2;
        public override Associativity Associativity => Associativity.Left;
        public override bool IsFunction => false;
        public override int ArgsCount => 2;
        public override double Execute(params double[] numbers)
        {
            if (numbers[1] == 0)
            {
                throw new DivideByZeroException("Division by zero.");
            }
            return numbers[0] / numbers[1];
        }
    }


    public class Power : Operation
    {
        public override string Name => "^";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => false;
        public override int ArgsCount => 2;
        public override double Execute(params double[] numbers)
        {
            return Math.Pow(numbers[0], numbers[1]);
        }
    }

    public class Sqrt : Operation
    {
        public override string Name => "sqrt";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 1;
        public override double Execute(params double[] numbers)
        {
            return Math.Sqrt(numbers[0]);
        }
    }

    public class Rt : Operation
    {
        public override string Name => "rt";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 2;

        public override double Execute(params double[] numbers)
        {
            if (numbers.Length != 2) throw new ArgumentException("Rt operation requires exactly two arguments.");
            return Math.Pow(numbers[1], 1 / numbers[0]);
        }
    }

    public class Sin : Operation
    {
        public override string Name => "sin";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 1;
        public override double Execute(params double[] numbers)
        {
            return Math.Sin(numbers[0]);
        }
    }

    public class Cos : Operation
    {
        public override string Name => "cos";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 1;
        public override double Execute(params double[] numbers)
        {
            return Math.Cos(numbers[0]);
        }
    }

    public class Tg : Operation
    {
        public override string Name => "tg";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 1;
        public override double Execute(params double[] numbers)
        {
            return Math.Tan(numbers[0]);
        }
    }

    public class Ctg : Operation
    {
        public override string Name => "ctg";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 1;
        public override double Execute(params double[] numbers)
        {
            return 1 / Math.Tan(numbers[0]);
        }
    }

    public class Log : Operation
    {
        public override string Name => "log";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 2;
        public override double Execute(params double[] numbers)
        {
            return Math.Log(numbers[1], numbers[0]);
        }
    }


    public class Ln : Operation
    {
        public override string Name => "ln";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 1;
        public override double Execute(params double[] numbers)
        {
            if (numbers.Length != 1) throw new ArgumentException("Ln operation requires exactly one argument.");
            return Math.Log(numbers[0]);
        }
    }

    public class Lg : Operation
    {
        public override string Name => "lg";
        public override int Priority => 3;
        public override Associativity Associativity => Associativity.Right;
        public override bool IsFunction => true;
        public override int ArgsCount => 1;
        public override double Execute(params double[] numbers)
        {
            if (numbers.Length != 1) throw new ArgumentException("Lg operation requires exactly one argument.");
            return Math.Log10(numbers[0]);
        }
    }
}
