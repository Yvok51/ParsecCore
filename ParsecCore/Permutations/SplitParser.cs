using ParsecCore.MaybeNS;

namespace ParsecCore.Permutations
{
    public class SplitParser<T, TInput>
    {
        public SplitParser(Parser<T, TInput> parser, IMaybe<T> defaultValue)
        {
            _parser = parser;
            _defaultValue = defaultValue;
        }

        public bool IsOptional { get => !_defaultValue.IsEmpty; }

        public Parser<T, TInput> Parser { get => _parser; }

        public T DefaultValue { get => _defaultValue.Value; }

        private readonly Parser<T, TInput> _parser;
        private readonly IMaybe<T> _defaultValue;
    }
}
