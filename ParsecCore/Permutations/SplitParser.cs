using ParsecCore.MaybeNS;

namespace ParsecCore.Permutations
{
    public class SplitParser<T, TInput>
    {
        internal SplitParser(Parser<T, TInput> parser, IMaybe<T> defaultValue)
        {
            _parser = parser;
            _defaultValue = defaultValue;
        }

        internal bool IsOptional { get => !_defaultValue.IsEmpty; }

        internal Parser<T, TInput> Parser { get => _parser; }

        internal T DefaultValue { get => _defaultValue.Value; }

        private readonly Parser<T, TInput> _parser;
        private readonly IMaybe<T> _defaultValue;
    }
}
