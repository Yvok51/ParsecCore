using ParsecCore.Indentation;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    public abstract class ErrorItem
    {
        public abstract int Size { get; }
    }

    public sealed class EndOfFile : ErrorItem
    {
        private EndOfFile() { }
        public override int Size => 1;

        public override string ToString()
        {
            return "\"end of file\"";
        }

        public static readonly EndOfFile Instance = new EndOfFile();
    }

    public sealed class StringToken : ErrorItem
    {
        public StringToken(string token)
        {
            _token = token;
        }

        public override string ToString()
        {
            return "\"" + _token + "\"";
        }

        public override int Size => _token.Length;

        private readonly string _token;
    }

    public sealed class Token<T> : ErrorItem
    {
        public Token(IReadOnlyList<T> token)
        {
            _tokens = token;
        }

        public override string ToString()
        {
            return string.Join(' ', _tokens.Select(item => "\"" + item.ToString() + "\""));
        }

        public override int Size => _tokens.Count;

        private readonly IReadOnlyList<T> _tokens;
    }

    public abstract class FancyError
    {
    }

    public class FailWithError : FancyError
    {
        public FailWithError(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return "\"" + _message + "\"";
        }

        private readonly string _message;
    }

    public class IndentationError : FancyError
    {
        public IndentationError(Relation relation, IndentLevel reference, IndentLevel actual)
        {
            _relation = relation;
            _reference = reference;
            _actual = actual;
        }

        public override string ToString()
        {
            return $"Incorrect indentation (expected indentation {_relation.ToPrettyString()} {_reference},"
                + $" encountered {_actual})";
        }

        private readonly Relation _relation;
        private readonly IndentLevel _reference;
        private readonly IndentLevel _actual;
    }

    public class ErrorCustom<T> : FancyError
    {
        public ErrorCustom(T item)
        {
            _item = item;
        }

        public override string ToString()
        {
            return "\"" + _item.ToString() + "\"";
        }

        private T _item;
    }
}
