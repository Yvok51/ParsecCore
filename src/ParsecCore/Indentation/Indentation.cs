using ParsecCore.Help;
using System;
using System.Collections.Generic;

namespace ParsecCore.Indentation
{
    public static class Indentation
    {
        /// <summary>
        /// Returns the position in the stream in a parser.
        /// Does not consume any input and cannot fail.
        /// </summary>
        /// <typeparam name="TInput"> The type of the input stream </typeparam>
        /// <returns> The position in the stream in a parser wrapper </returns>
        public static Parser<IndentLevel, TInput> Level<TInput>()
            => from pos in Parsers.Position<TInput>()
               select (IndentLevel)pos;

        /// <summary>
        /// Guard to check whether the indentation is how we want it
        /// </summary>
        /// <typeparam name="TSpace"> The type that the space consumer returns </typeparam>
        /// <typeparam name="TInput"> The type of the input stream </typeparam>
        /// <param name="spaceConsumer"> 
        /// Parser to consume any whitespace before the beggining of the line 
        /// </param>
        /// <param name="relation"> The relation between the reference and actual indentation we want </param>
        /// <param name="reference"> The reference indentation to compare against </param>
        /// <returns>
        /// The actual indentation if the indentation is correct
        /// or a parser that fails with incorrect indentation message
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<IndentLevel, TInput> Guard<TSpace, TInput>(
            Parser<TSpace, TInput> spaceConsumer,
            Relation relation,
            IndentLevel reference
        )
        {
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));

            return spaceConsumer.Then(
                Level<TInput>().Assert(
                    indent => relation.Satisfies(reference, indent),
                    (indent, pos) => new CustomError(
                            pos,
                            new IndentationError(relation, reference, indent)
                    )
                )
            );
        }

        /// <summary>
        /// Signify a parser that should not be indented (it should start on the first column)
        /// </summary>
        /// <typeparam name="TSpace"> The type that the space consumer returns </typeparam>
        /// <typeparam name="T"> The type the parser returns </typeparam>
        /// <typeparam name="TInput"> The type of the input stream </typeparam>
        /// <param name="spaceConsumer"> Parser which consumes any potential preceding whitespace </param>
        /// <param name="parser"> Parser that parses the non-intended content </param>
        /// <returns> The result of the <paramref name="parser"/> </returns>
        /// <exception cref="ArgumentNullException"> If any of the parameters are null </exception>
        public static Parser<T, TInput> NonIndented<TSpace, T, TInput>(
            Parser<TSpace, TInput> spaceConsumer,
            Parser<T, TInput> parser
        )
        {
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return Guard(spaceConsumer, Relation.EQ, IndentLevel.FirstPosition).Then(parser);
        }

        /// <summary>
        /// Parser for line folding. The user creates a parser which receives a space consumer. 
        /// This space consumer consumes whitespace between parts of the linefold.
        /// User further manually specifies how the statement can be folded.
        /// </summary>
        /// <typeparam name="T"> The return type of the line fold parser </typeparam>
        /// <typeparam name="TSpace"> Type returned by the space consumer </typeparam>
        /// <typeparam name="TInput"> Type of the input </typeparam>
        /// <param name="spaceConsumer">
        /// Parser which consumes whitespace between linefolds. Must consume newlines
        /// </param>
        /// <param name="linefoldParser">
        /// User's parser for parsing parts of the linefold seperated by a space consumer
        /// </param>
        /// <returns> Parser for parsing line folded statements </returns>
        public static Parser<T, TInput> LineFold<T, TSpace, TInput>(
            Parser<TSpace, TInput> spaceConsumer,
            Func<Parser<IndentLevel, TInput>, Parser<T, TInput>> linefoldParser
        )
        {
            return spaceConsumer.Then(
                   from indentLvl in Level<TInput>()
                   from res in linefoldParser(Guard(spaceConsumer, Relation.GT, indentLvl).Try())
                   select res);
        }

        /// <summary>
        /// Returns a parser which parses a head of a statement and
        /// afterward zero or more further indented items in subsequent lines.
        /// All of the items must start on the same indentation.
        /// The indentation is given by the first item in the block.
        /// <paramref name="spaceConsumer"/> is used to consume whitespace between lines and as such should consume
        /// newlines.
        /// </summary>
        /// <typeparam name="TSpace"> Type returned by the <paramref name="spaceConsumer"/> </typeparam>
        /// <typeparam name="TItem"> Type returned by the <paramref name="itemParser"/> </typeparam>
        /// <typeparam name="TReference"> Type returned by the parser of the reference item </typeparam>
        /// <typeparam name="TResult"> The return type of the parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="spaceConsumer"> Parser consuming whitespace </param>
        /// <param name="headParser"> Parser for the reference head item </param>
        /// <param name="itemParser"> Parser for individual items </param>
        /// <param name="transform">
        /// Function which transforms the reference item and list into the return type
        /// </param>
        /// <returns> Parser which parses a head and zero or more subsequent items at a greater indentation </returns>
        /// <exception cref="ArgumentNullException"> If any of the parameters are null </exception>
        public static Parser<TResult, TInput> BlockMany<TSpace, TItem, TReference, TResult, TInput>(
            Parser<TSpace, TInput> spaceConsumer,
            Parser<TReference, TInput> headParser,
            Parser<TItem, TInput> itemParser,
            Func<TReference, IList<TItem>, TResult> transform
        )
        {
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));
            if (headParser is null) throw new ArgumentNullException(nameof(headParser));
            if (transform is null) throw new ArgumentNullException(nameof(transform));
            if (itemParser is null) throw new ArgumentNullException(nameof(itemParser));

            return spaceConsumer.Then(
                   from referenceLvl in Level<TInput>()
                   from referenceItem in headParser // parse the head/reference item
                   from currLvl in Guard(spaceConsumer, Relation.GT, referenceLvl)
                        .Try().Optional() // check for any items
                   from eof in Parsers.IsEOF<TInput>()
                   from res in ParseItems(
                       eof, referenceLvl, referenceItem, currLvl, spaceConsumer, transform, itemParser
                       )
                   select res);

            static Parser<TResult, TInput> ParseItems(
                bool eof,
                IndentLevel reference,
                TReference referenceItem,
                Maybe<IndentLevel> current,
                Parser<TSpace, TInput> spaceConsumer,
                Func<TReference, IList<TItem>, TResult> transform,
                Parser<TItem, TInput> itemParser
            )
            {
                if (!eof && !current.IsEmpty) // an item found
                {
                    return from items in BlockItems(
                        reference, current.Value, spaceConsumer, itemParser)
                           select transform(referenceItem, items);
                }

                return spaceConsumer.MapConstant(transform(referenceItem, Array.Empty<TItem>())); // no items found
            }
        }

        /// <summary>
        /// Returns a parser which parses head of a statement and
        /// one or more further indented items in subsequent lines.
        /// All of the items must start on the same indentation.
        /// The indentation is given by the first parsed item
        /// <paramref name="spaceConsumer"/> is used to consume whitespace between lines and as such should consume
        /// newlines.
        /// </summary>
        /// <typeparam name="TSpace"> Type returned by the <paramref name="spaceConsumer"/> </typeparam>
        /// <typeparam name="TItem"> Type returned by the <paramref name="itemParser"/> </typeparam>
        /// <typeparam name="TReference"> Type returned by the parser of the reference item </typeparam>
        /// <typeparam name="TResult"> The return type of the parser </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="spaceConsumer"> Parser consuming whitespace </param>
        /// <param name="headParser"> Parser for the reference head item </param>
        /// <param name="itemParser"> Parser for individual items </param>
        /// <param name="transform">
        /// Function which transforms the reference item and list into the return type
        /// </param>
        /// <returns> Parser which parses a head and one or more subsequent items at a greater indentation </returns>
        /// <exception cref="ArgumentNullException"> If any of the parameters are null </exception>
        public static Parser<TResult, TInput> BlockMany1<TSpace, TItem, TResult, TReference, TInput>(
            Parser<TSpace, TInput> spaceConsumer,
            Parser<TReference, TInput> headParser,
            Parser<TItem, TInput> itemParser,
            Func<TReference, IList<TItem>, TResult> transform
        )
        {
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));
            if (headParser is null) throw new ArgumentNullException(nameof(headParser));
            if (transform is null) throw new ArgumentNullException(nameof(transform));
            if (itemParser is null) throw new ArgumentNullException(nameof(itemParser));

            return spaceConsumer.Then(
                   from referenceLvl in Level<TInput>()
                   from head in headParser
                   from currPosition in Guard(spaceConsumer, Relation.GT, referenceLvl)
                   from _ in Guard(spaceConsumer, Relation.EQ, currPosition)
                   from firstItem in itemParser
                   from items in BlockItems(referenceLvl, currPosition, spaceConsumer, itemParser)
                   select transform(head, items.Prepend(firstItem)));
        }

        /// <summary>
        /// Returns a parser which parses many indented items in a row.
        /// All of the items must follow the same relation compared to the <paramref name="reference"/> indentation.
        /// </summary>
        /// <typeparam name="TItem"> The type of the parsed items </typeparam>
        /// <typeparam name="TSpace"> The parsed whitespace type </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="reference"> The reference indentation level to compare against </param>
        /// <param name="relation">
        /// The relation the identation of the items must hold when compared to the <paramref name="reference"/>
        /// </param>
        /// <param name="spaceConsumer"> Parser to consume whitespace </param>
        /// <param name="itemParser"> Parser for the individual items </param>
        /// <returns> Parser which parses many items whose indentation satisfies the given relation </returns>
        public static Parser<IReadOnlyList<TItem>, TInput> Many<TItem, TSpace, TInput>(
            IndentLevel reference,
            Relation relation,
            Parser<TSpace, TInput> spaceConsumer,
            Parser<TItem, TInput> itemParser
        )
        {
            var lineBeginningParser = from position in spaceConsumer.Then(Level<TInput>())
                                      from end in Parsers.IsEOF<TInput>()
                                      select (position, end);
            var optItemParser = itemParser.Optional();

            return (input) =>
            {
                List<TItem> items = new();
                List<IResult<Maybe<TItem>, TInput>> results = new();

                while (true)
                {
                    var res = lineBeginningParser(input);
                    input = res.UnconsumedInput;
                    if (res.IsError)
                    {
                        return Result.Failure<List<TItem>, Maybe<TItem>, (IndentLevel, bool), TInput>(
                            results, res, input
                        );
                    }
                    var (position, end) = res.Result;

                    if (end) // end of input
                    {
                        return Result.Success(items, results, input);
                    }

                    var itemRes = optItemParser(input);
                    input = itemRes.UnconsumedInput;
                    results.Add(itemRes);
                    if (itemRes.IsError)
                    {
                        return Result.Failure<List<TItem>, Maybe<TItem>, TInput>(results);
                    }
                    else if (itemRes.Result.IsEmpty)
                    {
                        return Result.Success(items, results, input);
                    }
                    else if (!relation.Satisfies(reference, position))
                    {
                        return Result.Failure<List<TItem>, Maybe<TItem>, TInput>(
                            results,
                            new CustomError(input.Position, new IndentationError(relation, reference, position)),
                            input
                        );
                    }
                    items.Add(itemRes.Result.Value);
                }
            };
        }

        /// <summary>
        /// Returns a parser which parses many indented items in a row.
        /// All of the items must follow the same relation compared to the <paramref name="reference"/> indentation.
        /// At least one item has to be parsed.
        /// </summary>
        /// <typeparam name="TItem"> The type of the parsed items </typeparam>
        /// <typeparam name="TSpace"> The parsed whitespace type </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="reference"> The reference indentation level to compare against </param>
        /// <param name="relation">
        /// The relation the identation of the items must hold when compared to the <paramref name="reference"/>
        /// </param>
        /// <param name="spaceConsumer"> Parser to consume whitespace </param>
        /// <param name="itemParser"> Parser for the individual items </param>
        /// <returns> Parser which parses many items whose indentation satisfies the given relation </returns>
        public static Parser<IReadOnlyList<TItem>, TInput> Many1<TItem, TSpace, TInput>(
            IndentLevel reference,
            Relation relation,
            Parser<TSpace, TInput> spaceConsumer,
            Parser<TItem, TInput> itemParser
        )
        {
            return from _ in Guard(spaceConsumer, relation, reference)
                   from first in itemParser
                   from rest in Many(reference, relation, spaceConsumer, itemParser)
                   select rest.Prepend(first);
        }

        /// <summary>
        /// Optionally applies a parser with indentation checking.
        /// Tries to apply <paramref name="parser"/>.
        /// If it succeeds then check that its indentation follows the given 
        /// <paramref name="relation"/> compared to <paramref name="reference"/>.
        /// If the <paramref name="parser"/> fails but does not consume input,
        /// then this parser succeeds and returns empty <see cref="Maybe{T}"/>.
        /// If it fails and consumes input, then this entire parser fails.
        /// </summary>
        /// <typeparam name="T"> The output type of <paramref name="parser"/> </typeparam>
        /// <typeparam name="TInput"> The input type of <paramref name="parser"/> </typeparam>
        /// <param name="reference"> The reference indentation to compare against </param>
        /// <param name="relation"> The relation to hold when compared to <paramref name="reference"/> </param>
        /// <param name="parser"> The parser to try to apply </param>
        /// <returns>
        /// Parser which optionally applies <paramref name="parser"/> and also checks its indentation
        /// </returns>
        public static Parser<Maybe<T>, TInput> Optional<T, TInput>(
            IndentLevel reference,
            Relation relation,
            Parser<T, TInput> parser
        )
        {
            return (input) =>
            {
                var indentation = Level<TInput>()(input).Result;
                var result = parser(input);
                if (result.IsResult && relation.Satisfies(reference, indentation))
                {
                    return Result.Success(Maybe.FromValue(result.Result), result);
                }
                if (result.IsError)
                {
                    if (input.Equals(result.UnconsumedInput))
                    {
                        return Result.Success(Maybe.Nothing<T>(), result);
                    }

                    return Result.RetypeError<T, Maybe<T>, TInput>(result);
                }
                return Result.Failure<Maybe<T>, TInput>(
                    new CustomError(input.Position, new IndentationError(relation, reference, indentation)),
                    result.UnconsumedInput
                );
            };
        }

        /// <summary>
        /// Parse a list of lines which occur on the same indentation.
        /// Presumes the <paramref name="reference"/> indentation is less than the <paramref name="required"/>
        /// indentation. The <paramref name="spaceConsumer"/> is used to consume whitespace between items and
        /// as such should consume newlines.
        /// </summary>
        /// <typeparam name="TSpace"> Type returned by the <paramref name="spaceConsumer"/> </typeparam>
        /// <typeparam name="TItem"> Type returned by the <paramref name="itemParser"/> </typeparam>
        /// <typeparam name="TInput"> The input type of the parsers </typeparam>
        /// <param name="reference"> Reference indentation level of the head of the block </param>
        /// <param name="required"> The required indentation of the items </param>
        /// <param name="spaceConsumer"> Parser consuming whitespace </param>
        /// <param name="itemParser"> Parser for individual items </param>
        /// <returns> Parser which parsers a list of items all of which are on the same indentation </returns>
        private static Parser<List<TItem>, TInput> BlockItems<TSpace, TItem, TInput>(
            IndentLevel reference,
            IndentLevel required,
            Parser<TSpace, TInput> spaceConsumer,
            Parser<TItem, TInput> itemParser
        )
        {
            var lineBeginningParser = from position in spaceConsumer.Then(Level<TInput>())
                                      from end in Parsers.IsEOF<TInput>()
                                      select (position, end);

            return (input) =>
            {
                List<TItem> items = new();
                List<IResult<TItem, TInput>> results = new();

                while (true)
                {
                    var res = lineBeginningParser(input);
                    input = res.UnconsumedInput;
                    if (res.IsError)
                    {
                        return Result.Failure<List<TItem>, TItem, (IndentLevel, bool), TInput>(results, res, input);
                    }
                    var (position, end) = res.Result;

                    if (end || position <= reference) // item is not indented
                    {
                        return Result.Success(items, results, input);
                    }
                    else if (position == required)
                    {
                        var itemRes = itemParser(input);
                        input = itemRes.UnconsumedInput;
                        results.Add(itemRes);
                        if (itemRes.IsError)
                        {
                            return Result.Failure<List<TItem>, TItem, TInput>(results);
                        }

                        items.Add(itemRes.Result);
                    }
                    else
                    {
                        return Result.Failure<List<TItem>, TItem, TInput>(
                            results,
                            new CustomError(input.Position, new IndentationError(Relation.EQ, required, position)),
                            input
                        );
                    }
                }
            };
        }
    }
}
