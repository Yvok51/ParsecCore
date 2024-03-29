﻿using ParsecCore.Indentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    /// <summary>
    /// Items located in <see cref="ParseError"/>
    /// </summary>
    public abstract class ErrorItem
    {
        public abstract int Size { get; }
    }

    /// <summary>
    /// Represents an end of file error, i.e. we have encountered an unexpected end of file
    /// or we expected an end of file.
    /// </summary>
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

    /// <summary>
    /// A character in the input
    /// </summary>
    public sealed class CharToken : ErrorItem, IEquatable<CharToken>
    {
        public CharToken(char c)
        {
            _c = c;
        }

        public override string ToString()
        {
            return string.Concat("'", _c.ToString(), "'");
        }


        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as StringToken);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_c);
        }

        public bool Equals(CharToken? other)
        {
            return other is not null && _c == other._c;
        }

        public override int Size => 1;

        private readonly char _c;
    }

    /// <summary>
    /// General message located in the error
    /// </summary>
    public sealed class StringToken : ErrorItem, IEquatable<StringToken>
    {
        public StringToken(string token)
        {
            _token = token;
        }

        public override string ToString()
        {
            return string.Concat("\"", _token, "\"");
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

    /// <summary>
    /// A general token located in the error
    /// </summary>
    /// <typeparam name="T"> The type of token </typeparam>
    public sealed class ListToken<T> : ErrorItem, IEquatable<ListToken<T>>
    {
        public ListToken(IReadOnlyList<T> tokens)
        {
            _tokens = tokens;
        }

        public override string ToString()
        {
            return string.Join(", ", _tokens.Select(item => "\"" + item?.ToString() + "\""));
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as ListToken<T>);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_tokens);
        }

        public bool Equals(ListToken<T>? other)
        {
            return other is not null && Enumerable.SequenceEqual(other._tokens, _tokens);
        }

        public override int Size => _tokens.Count;

        private readonly IReadOnlyList<T> _tokens;
    }

    public sealed class SingleToken<T> : ErrorItem, IEquatable<SingleToken<T>>
    {
        public SingleToken(T token)
        {
            _token = token;
        }

        public override string ToString()
        {
            return string.Concat("'", _token!.ToString(), "'");
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as SingleToken<T>);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_token);
        }

        public bool Equals(SingleToken<T>? other)
        {
            return other is not null && other._token is not null && other._token.Equals(_token);
        }

        public override int Size => 1;

        private readonly T _token;
    }

    /// <summary>
    /// A fancy error item which is used in <see cref="CustomError"/>
    /// </summary>
    public abstract class CustomErrorItem
    {
    }

    /// <summary>
    /// A message used in <see cref="Parsers.Fail{T, TInputToken}(string)"/> and
    /// <see cref="Parsers.FailWith{T, TInputToken}(Parser{T, TInputToken}, string)"/>
    /// </summary>
    public class FailError : CustomErrorItem, IEquatable<FailError>
    {
        public FailError(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return "\"" + _message + "\"";
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && Equals(obj as FailError);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_message);
        }

        public bool Equals(FailError? other)
        {
            return other is not null && other.Equals(other._message);
        }

        private readonly string _message;
    }

    /// <summary>
    /// Indentation error used by the indentation module.
    /// </summary>
    public class IndentationError : CustomErrorItem, IEquatable<IndentationError>
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

    /// <summary>
    /// Custom error that the user can specify
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ErrorCustom<T> : CustomErrorItem, IEquatable<ErrorCustom<T>>
    {
        public ErrorCustom(T item)
        {
            _item = item;
        }

        public override string ToString()
        {
            return "\"" + _item?.ToString() + "\"";
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
            return other is not null && _item is not null && _item.Equals(other._item);
        }

        private T _item;
    }
}
