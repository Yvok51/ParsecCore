using ParsecCore.Help;
using ParsecCore.MaybeNS;
using System;
using System.Collections.Generic;

namespace ParsecCore.Indentation
{
    public static class Indentation
    {
        /// <summary>
        /// Returns the position in the stream in a parser.
        /// Useful for use in the context of LINQ
        /// </summary>
        /// <typeparam name="TInput"> The type of the input stream </typeparam>
        /// <returns> The position in the stream in a parser wrapper </returns>
        public static Parser<IndentLevel, TInput> IndentationLevel<TInput>()
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
        public static Parser<IndentLevel, TInput> IndentationGuard<TSpace, TInput>(
            Parser<TSpace, TInput> spaceConsumer,
            Relation relation,
            IndentLevel reference
        )
        {
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));

            return spaceConsumer.Then(
                IndentationLevel<TInput>().Assert(
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

            return IndentationGuard(spaceConsumer, Relation.EQ, IndentLevel.FirstPosition).Then(parser);
        }

        /// <summary>
        /// Parser for line folding. The user creates a parser which receives a space consumer which consumes
        /// whitespace between parts of the linefold and manually specifies how the statement can be folded.
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
                   from indentLvl in IndentationLevel<TInput>()
                   from res in linefoldParser(IndentationGuard(spaceConsumer, Relation.GT, indentLvl))
                   select res);
        }

        /// <summary>
        /// Returns a parser which parses zero or more items in subsequent lines.
        /// All of the items must start on the same indentation which given by <paramref name="desiredIndentation"/>.
        /// <paramref name="spaceConsumer"/> is used to consume whitespace between lines and as such should consume
        /// newlines.
        /// </summary>
        /// <typeparam name="TSpace"> Type returned by the <paramref name="spaceConsumer"/> </typeparam>
        /// <typeparam name="TItem"> Type returned by the <paramref name="itemParser"/> </typeparam>
        /// <typeparam name="TReference"> Type returned by the parser of the reference item </typeparam>
        /// <typeparam name="TResult"> The return type of the parser </typeparam>
        /// <param name="spaceConsumer"> Parser consuming whitespace </param>
        /// <param name="referenceParser"> Parser for the reference item </param>
        /// <param name="desiredIndentation">
        /// The desired indentation of the lines. If it is empty the desired indentation is taken as the indentation
        /// of the first line
        /// </param>
        /// <param name="transform">
        /// Function which transforms the reference item and list into the return type
        /// </param>
        /// <param name="itemParser"> Parser for individual items </param>
        /// <returns> Parser which parses a head and zero or more subsequent items at a greater indentation </returns>
        /// <exception cref="ArgumentNullException"> If any of the parameters are null </exception>
        public static Parser<TResult, char> IndentationBlockMany<TSpace, TItem, TReference, TResult>(
            Parser<TSpace, char> spaceConsumer,
            Parser<TReference, char> referenceParser,
            Maybe<IndentLevel> desiredIndentation,
            Func<TReference, IList<TItem>, TResult> transform,
            Parser<TItem, char> itemParser
        )
        {
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));
            if (referenceParser is null) throw new ArgumentNullException(nameof(referenceParser));
            if (transform is null) throw new ArgumentNullException(nameof(transform));
            if (itemParser is null) throw new ArgumentNullException(nameof(itemParser));

            return spaceConsumer.Then(
                   from referenceLvl in IndentationLevel<char>()
                   from referenceItem in referenceParser // parse the head/reference item
                   from lvl in IndentationGuard(spaceConsumer, Relation.GT, referenceLvl)
                        .Try().Optional() // check for any items
                   from eof in Parsers.IsEOF<char>()
                   from res in ParseItems(
                       eof, referenceLvl, referenceItem, lvl, spaceConsumer, desiredIndentation, transform, itemParser
                       )
                   select res);

            static Parser<TResult, char> ParseItems(
                bool eof,
                IndentLevel reference,
                TReference referenceItem,
                Maybe<IndentLevel> current,
                Parser<TSpace, char> spaceConsumer,
                Maybe<IndentLevel> desiredIndentation,
                Func<TReference, IList<TItem>, TResult> transform,
                Parser<TItem, char> itemParser
            )
            {
                if (!eof && !current.IsEmpty) // an item found
                {
                    return from items in IndentedItems(
                        reference, desiredIndentation.Else(current.Value), spaceConsumer, itemParser)
                           select transform(referenceItem, items);
                }

                return spaceConsumer.MapConstant(transform(referenceItem, Array.Empty<TItem>())); // no items found
            }
        }

        /// <summary>
        /// Returns a parser which parses one or more items in subsequent lines.
        /// All of the items must start on the same indentation which given by <paramref name="desiredIndentation"/>.
        /// <paramref name="spaceConsumer"/> is used to consume whitespace between lines and as such should consume
        /// newlines.
        /// </summary>
        /// <typeparam name="TSpace"> Type returned by the <paramref name="spaceConsumer"/> </typeparam>
        /// <typeparam name="TItem"> Type returned by the <paramref name="itemParser"/> </typeparam>
        /// <typeparam name="TReference"> Type returned by the parser of the reference item </typeparam>
        /// <typeparam name="TResult"> The return type of the parser </typeparam>
        /// <param name="spaceConsumer"> Parser consuming whitespace </param>
        /// <param name="referenceParser"> Parser for the reference item </param>
        /// <param name="desiredIndentation">
        /// The desired indentation of the lines. If it is empty the desired indentation is taken as the indentation
        /// of the first line
        /// </param>
        /// <param name="transform">
        /// Function which transforms the reference item and list into the return type
        /// </param>
        /// <param name="itemParser"> Parser for individual items </param>
        /// <returns> Parser which parses a head and one or more subsequent items at a greater indentation </returns>
        /// <exception cref="ArgumentNullException"> If any of the parameters are null </exception>
        public static Parser<TResult, char> IndentationBlockMany1<TSpace, TItem, TResult, TReference>(
            Parser<TSpace, char> spaceConsumer,
            Parser<TReference, char> referenceParser,
            Maybe<IndentLevel> desiredIndentation,
            Func<TReference, IList<TItem>, TResult> transform,
            Parser<TItem, char> itemParser
        )
        {
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));
            if (referenceParser is null) throw new ArgumentNullException(nameof(referenceParser));
            if (transform is null) throw new ArgumentNullException(nameof(transform));
            if (itemParser is null) throw new ArgumentNullException(nameof(itemParser));

            return spaceConsumer.Then(
                   from referenceLvl in IndentationLevel<char>()
                   from referenceItem in referenceParser
                   from currPosition in IndentationGuard(spaceConsumer, Relation.GT, referenceLvl
                       ).Assert( // the first item is more indented than the reference item
                        curr => curr > referenceLvl,
                        (curr, pos) => new CustomError(pos, new IndentationError(Relation.GT, referenceLvl, curr))
                       ).Assert( // If desiredIndent is specified, then it is complied with
                        curr => desiredIndentation.IsEmpty || curr == desiredIndentation.Value,
                        (curr, pos) => 
                            new CustomError(pos, new IndentationError(Relation.EQ, desiredIndentation.Value, curr))
                       )
                   from firstItem in itemParser
                   from items in IndentedItems(referenceLvl, desiredIndentation.Else(currPosition), spaceConsumer, itemParser)
                   select transform(referenceItem, items.Prepend(firstItem)));
        }

        /// <summary>
        /// Parse a list of lines which occur on the same indentation.
        /// Presumes the <paramref name="reference"/> indentation is less than the <paramref name="required"/>
        /// indentation. The <paramref name="spaceConsumer"/> is used to consume whitespace between items and
        /// as such should consume newlines.
        /// </summary>
        /// <typeparam name="TSpace"> Type returned by the <paramref name="spaceConsumer"/> </typeparam>
        /// <typeparam name="TItem"> Type returned by the <paramref name="itemParser"/> </typeparam>
        /// <param name="reference"> Reference indentation level of the head of the block </param>
        /// <param name="required"> The required indentation of the items </param>
        /// <param name="spaceConsumer"> Parser consuming whitespace </param>
        /// <param name="itemParser"> Parser for individual items </param>
        /// <returns> Parser which parsers a list of items all of which are on the same indentation </returns>
        private static Parser<List<TItem>, char> IndentedItems<TSpace, TItem>(
            IndentLevel reference,
            IndentLevel required,
            Parser<TSpace, char> spaceConsumer,
            Parser<TItem, char> itemParser
        )
        {
            var lineBeginningParser = from _ in spaceConsumer
                                      from position in IndentationLevel<char>()
                                      from end in Parsers.IsEOF<char>()
                                      select (position, end);

            return (input) =>
            {
                List<TItem> items = new();

                while (true)
                {
                    var res = lineBeginningParser(input);
                    input = res.UnconsumedInput;
                    if (res.IsError)
                    {
                        return Result.RetypeError<(IndentLevel, bool), List<TItem>, char>(res);
                    }
                    var (position, end) = res.Result;

                    if (end || position <= reference) // item is not indented
                    {
                        return Result.Success(items, input);
                    }
                    else if (position == required)
                    {
                        var itemRes = itemParser(input);
                        input = itemRes.UnconsumedInput;
                        if (itemRes.IsError)
                        {
                            return Result.RetypeError<TItem, List<TItem>, char>(itemRes);
                        }

                        items.Add(itemRes.Result);
                    }
                    else
                    {
                        return Result.Failure<List<TItem>, char>(
                            new CustomError(input.Position, new IndentationError(Relation.EQ, required, position)),
                            input
                        );
                    }
                }
            };
        }
    }
}
