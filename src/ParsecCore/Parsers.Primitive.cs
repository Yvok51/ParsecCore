using ParsecCore.Input;
using ParsecCore.ParsersHelp;
using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Parser which parses an end of file - end of input.
        /// If the input has not ended, then fails.
        /// Used to check we parsed the entire input.
        /// <see cref="None"/> is a helper struct which is empty - it only serves the type system
        /// </summary>
        /// <typeparam name="TInput"> The token type of the input </typeparam>
        /// <returns> Parser which parses an end of file </returns>
        public static Parser<None, TInput> EOF<TInput>()
            => EOFParser.Parser<TInput>();

        /// <summary>
        /// Parser which answers whether we have reached the end of file.
        /// It always succeeds and returns a boolean value signifying the answer.
        /// True - there is no more input. False - there is more input yet.
        /// </summary>
        /// <typeparam name="TInput"> The token type of the input </typeparam>
        /// <returns> Boolean value whether we are at the end of file </returns>
        public static Parser<bool, TInput> IsEOF<TInput>() =>
            from end in EOF<TInput>().Optional()
            select !end.IsEmpty;

        /// <summary>
        /// Return a parser which does not consume any input and only returns the value given
        /// </summary>
        /// <typeparam name="T"> The type of the value the parser returns </typeparam>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <param name="value"> The value for the parser to return </param>
        /// <returns> Parser which returns the given value </returns>
        public static Parser<T, TInputToken> Return<T, TInputToken>(T value)
        {
            return (input) =>
            {
                return Result.Success(value, input);
            };
        }

        /// <summary>
        /// Parser which returns the current position of the input
        /// </summary>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <returns></returns>
        public static Parser<Position, TInputToken> Position<TInputToken>()
        {
            return (input) =>
            {
                return Result.Success(input.Position, input);
            };
        }

        /// <summary>
        /// Make the creation of the parser indirect.
        /// <para/>
        /// Used when a circular compile time dependency occurs between the parsers. 
        /// In such a case value of one of the parsers is always initialized only after being used in another parser.
        /// The value is thus null and an error occurs.
        /// <para/>
        /// We solve this by indirectly initializing one of the parsers, thus the value of the parser with whom
        /// we are circularly dependent is taken after it has been initialized and thus everything works.
        /// We do this by putting the parser into a lambda function. We postpone the creation of the parser to the
        /// runtime and therefore the other parser on which we are dependent is already defined.
        /// <para/>
        /// Error will occur if the grammar with the circularly dependent parsers is left recursive. In that case
        /// the indirection will go on until a stack overflow.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <param name="getParser"> Function to get the parser from </param>
        /// <returns> Same parser as returned by <paramref name="getParser"/> only behaving correctly </returns>
        /// <exception cref="ArgumentNullException"> If provided function is null </exception>
        public static Parser<T, TInputToken> Indirect<T, TInputToken>(
            Func<Parser<T, TInputToken>> getParser
        )
        {
            if (getParser is null) throw new ArgumentNullException(nameof(getParser));

            return (input) => getParser()(input);
        }
    }
}
