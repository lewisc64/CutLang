namespace CutLang.Token
{
    public class NumberToken : IToken
    {
        public double Value { get; }

        public NumberToken(double value)
        {
            Value = value;
        }
    }
}
