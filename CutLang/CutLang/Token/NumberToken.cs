namespace CutLang.Token
{
    public class DecimalNumberToken : IToken
    {
        public double Value { get; }

        public DecimalNumberToken(double value)
        {
            Value = value;
        }
    }

    public class IntegerNumberToken : IToken
    {
        public int Value { get; }

        public IntegerNumberToken(int value)
        {
            Value = value;
        }
    }
}
