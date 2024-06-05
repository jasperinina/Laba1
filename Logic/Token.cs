namespace Logic
{
    public abstract class Token
    {
    }

    public class Variable : Token
    {
        public string Symbol { get; set; }
        public override string ToString()
        {
            return Symbol;
        }
    }

    public class Parenthesis : Token
    {
        public char Value { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Number : Token
    {
        public double Value { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}