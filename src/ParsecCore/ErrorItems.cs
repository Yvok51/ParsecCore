using ParsecCore.Indentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    public abstract class ErrorItem
    {
        public abstract int Size { get; }
    }

    public sealed class EndOfFile : ErrorItem, IEquatable<EndOfFile>
    {
        private EndOfFile() { }
        public override int Size => 1;

        public override string ToString()
        {
            return "\"end of file\"";
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as EndOfFile);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(EndOfFile? other)
        {
            return true;
        }

        public static readonly EndOfFile Instance = new EndOfFile();
    }

    public sealed class StringToken : ErrorItem, IEquatable<StringToken>
    {
        public StringToken(string token)
        {
            _token = token;
        }

        public override string ToString()
        {
            return "\"" + _token + "\"";
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as StringToken);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_token);
        }

        public bool Equals(StringToken? other)
        {
            return other is not null && _token.Equals(other._token);
        }

        public override int Size => _token.Length;

        private readonly string _token;
    }

    public sealed class Token<T> : ErrorItem, IEquatable<Token<T>>
    {
        public Token(IReadOnlyList<T> token)
        {
            _tokens = token;
        }

        public override string ToString()
        {
            return string.Join(' ', _tokens.Select(item => "\"" + item.ToString() + "\""));
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as Token<T>);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_tokens);
        }

        public bool Equals(Token<T>? other)
        {
            return other is not null && Enumerable.SequenceEqual(other._tokens, _tokens);
        }

        public override int Size => _tokens.Count;

        private readonly IReadOnlyList<T> _tokens;
    }

    public abstract class FancyError
    {
    }

    public class FailWithError : FancyError, IEquatable<FailWithError>
    {
        public FailWithError(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return "\"" + _message + "\"";
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as FailWithError);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_message);
        }

        public bool Equals(FailWithError? other)
        {
            return other is not null && other.Equals(other._message);
        }

        private readonly string _message;
    }

    public class IndentationError : FancyError, IEquatable<IndentationError>
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

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as IndentationError);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_relation, _reference, _actual);
        }

        public bool Equals(IndentationError? other)
        {
            return other is not null 
                && _relation.Equals(other._relation)
                && _reference.Equals(other._reference)
                && _actual.Equals(other._actual);
        }

        private readonly Relation _relation;
        private readonly IndentLevel _reference;
        private readonly IndentLevel _actual;
    }

    public class ErrorCustom<T> : FancyError, IEquatable<ErrorCustom<T>>
    {
        public ErrorCustom(T item)
        {
            _item = item;
        }

        public override string ToString()
        {
            return "\"" + _item.ToString() + "\"";
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as ErrorCustom<T>);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_item);
        }

        public bool Equals(ErrorCustom<T>? other)
        {
            return other is not null && _item.Equals(other._item);
        }

        private T _item;
    }
}
