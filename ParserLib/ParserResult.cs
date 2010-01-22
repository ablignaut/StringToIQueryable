
namespace ParserLib
{
    public sealed class ParserResult<T>
    {
        public ParserResult(T parsed, string remaining)
        {
            Parsed = parsed;
            Remaining = remaining;
        }

        public T Parsed { get; private set; }
        public string Remaining { get; private set; }
    }
}
