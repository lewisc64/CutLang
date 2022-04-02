using CutLang.Token;

namespace CutLang.Extensions
{
    public static class ITokenExtensions
    {
        public static bool OperationCanProduceVideo(this IToken token)
        {
            return token.GetType().IsAssignableTo(typeof(IProducesVideo));
        }
    }
}
