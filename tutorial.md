# Tutorial

In this tutorial we will explain how to use the ParsecCore library.
We will mention general guidelines and recommendations to get the most out of our library.
We will explain how the library works through examples.
Unless explicitly stated, all mentioned classes are located in the `ParsecCore` namespace.

## Navigation

- [Core library](#core-library)
  - [Creating parser input](#creating-parser-input)
  - [Simple parsers](#simple-parsers)
  - [The result of parsing](#the-result-of-parsing)
  - [Sequencing parsers](#sequencing-parsers)
    - [Parser LINQ](#parser-linq)
    - [Then and FollowedBy](#then-and-followedby)
    - [All and String](#all-and-string)
  - [Choice](#choice)
  - [Lookahead](#lookahead)
  - [Errors](#errors)
  - [Maybe](#maybe)
  - [Other combinators](#other-combinators)
    - [Indirect](#indirect)
- [Extensions](#extensions)
  - [Expressions](#expressions)
  - [Permutations](#permutations)
  - [Indentation](#indentation)
    - [NonIndented](#nonindented)
    - [BlockMany](#blockmany)
    - [Many](#many)
    - [Optional](#optional)
    - [Linefold](#linefold)
- [Advice on writing parsers](#advice-for-writing-parsers)
  - [Create parsers only once](#create-parsers-only-once)
  - [Avoid backtracking](#avoid-backtracking)
  - [Parse whitespace after each element](#parse-whitespace-after-each-element)

## Core library

The core library provides the ability to create parsers using parser combinators.
It also gives us a base upon which other extensions can build.
Unless stated otherwise, the parsers and parser combinators provided in this section are located in the `Parsers` class.

[↩](#navigation)

### Creating parser input

Before we start the actual parsing, we need to create the parser input.
We can create parser inputs from a string, a list of tokens, and a stream that allows seeking.
All of these are created through the `ParserInput.Create` methods.
These methods have all of the relevant overloads for creating the parser input.

The `updatePosition` parameter defines how the parser input keeps track of its position.
It is called whenever an input token is read and determines, based on the token, what the next position should be.
If we were reading characters, then an implementation could look like the following example.
The `tabSize` is a captured value.

```csharp
(readChar, position) =>
{
    return readChar switch
    {
        '\n' => position.WithNewLine(),
        '\t' => position.WithTab(tabSize),
        _ => position.WithIncreasedColumn()
    };
};
```

The parser inputs for strings and character streams have a default implementation of `updatePosition`.
It is equivalent to the example above.
For lists of other tokens, the `updatePosition` function has to be supplied manually.
As this function is called whenever we advance in the parser input, we recommend not including any complex expressions or statements.

[↩](#navigation)

### Simple parsers

The simplest parsers contained in ParsecCore are parsers for single characters.
The two most common are the parsers `Satisfy` and `Char`.
The `Satisfy` parser parses a single symbol and tests it against a predicate.
The `Char` parser is a specialized version of `Satisfy` that tests whether the parsed symbol is equal to a given symbol
The simplest example we can show is the parsing of a single character.

```csharp
IParserInput<char> input = ParserInput.Create("abc");
Parser<char, char> parser = Parsers.Char('a');

IResult<char, char> result = parser(input);
```

This example also displays a general outline of working with ParsecCore.
We create the parser input.
Then we prepare the parser with which we will work.
Finally, we attempt to parse the input with the prepared parser.
In this case, the parser succeeds, and its result contains the \emph{output} of the parse, the character `'a'`.

The delegate type `Parser` and the interface `IResult` have two generic type arguments.
The first argument is the output type of the parse.
The second argument is the type of the symbols that serve as our input.

Another important primitive parser is the `EOF` parser.
This parser succeeds if we are at the end of a file.
Otherwise, it fails.
We use this parser to check that the entire parser input has been processed.

[↩](#navigation)

### The result of parsing

Every parse attempt creates an `IResult` object.
This object can have two forms, either it can signify a successful parse, or it can represent a failed parse.
We can ask the `IResult` object which outcome it embodies.
The property `IsResult` answers whether the parse was successful, and the property `IsError` tells us if the parse was a failure.
Depending on which type of result it is, we can then access either `Result` or `Error`.

```csharp
Parser<char, char> parser = Parsers.Char('a');
IParserInput<char> input = ParserInput.Create("abc");

IResult<char, char> result = parser(input);
if (result.IsResult) // Same as !result.IsError
{
    Console.WriteLine("Great, the parse was successful!");
    Console.WriteLine(result.Result);
}
else
{
    Console.Writeline("An error occured :(");
    Console.WriteLine(result.Error);
}
```

In both cases, the `IResult` object contains the remaining output that has not been processed yet.
In this instance, it is equivalent to the string `"bc"`.
We can access this output through the `UnconsumedInput` property.

[↩](#navigation)

### Sequencing parsers

The ability to parse a single character is useful.
However, in most cases, we need quite a bit more.
So far, we have parsed only a single character at a time.
Now, we want to parse a string of characters.

The first way we might think of to parse a string is by chaining calls to parsers together.
Imagine we want to parse the string `"abc"`.
With the stated method, we might come up with something like this.

```csharp
var input = ParserInput.Create("abc");

var resultA = Parsers.Char('a')(input);
if (resultA.IsError)
{
    return resultA.Error.ToString();
}
var resultB = Parsers.Char('b')(resultA.UnconsumedInput);
if (resultB.IsError)
{
    return resultB.Error.ToString();
}
var resultC = Parsers.Char('c')(resultB.UnconsumedInput);
if (resultC.IsError)
{
    return resultC.Error.ToString();
}
return "abc";
```

We chain if statements together until all parsers have succeeded or one of them fails.
This solution is verbose, inflexible, and prone to errors.
Thankfully, we have a solution.

[↩](#navigation)

#### Parser LINQ

The ParsecCore implements the `Select`, `SelectMany`, and `Where` extension methods and thus allows the use of LINQ query syntax for parsers.
For a quick explanation of LINQ, see [here](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/).
The above example can be rewritten into a LINQ query in the following way.

```csharp
var input = ParserInput.Create("abc");
// Definition of a new parser
Parser<string, char> parserABC =
    from a in Parsers.Char('a') // Parse character 'a'
    from b in Parsers.Char('b') // Parse character 'b'
    from c in Parsers.Char('c') // Parse character 'c'
    select "abc"; // if parses successful, return "abc"

var result = parserABC(input);
if (result.IsResult)
{
    return result.Result;
}
return result.Error.ToString();
```

It is important to note that, in this example, we are creating a new parser.
A parser query can take multiple parsers and combine them into a new one.
In this example, we are combining the parsers for characters `'a'`, `'b'`, and `'c'`.
If they are all successful, the newly created parser outputs the string `"abc"`.
Otherwise, the new parser returns the error of the first parser that failed.
The query syntax for parsers has four possible clauses.
Here, we use the two most important.

The clause beginning with `from` represents an attempt to parse the input.
From now, we will refer to this clause as the \emph{from} clause.
It has two parts.
The first is the variable name after `from`.
This variable stores the output of the parse if it is successful.
It can be used further in the query.
The second part is the parser located after `in`.
This section defines with which parser we will attempt to parse.

The second clause we use is the \emph{select} clause.
This clause tells the parser what to return if all the previous parses are successful.
Every parser query has to have precisely one select clause, and this clause has to be the last one in the query.

As we mentioned before, we can use the output of a previous parse in the following clauses.
Say we want to parse a sequence of three of the same letter --- the strings `"aaa"` or `"ZZZ"`, for instance.
The parser for a single letter is `Parsers.Letter`.
We can thus define the parser for three letters in this manner.

```csharp
Parser<string, char> threeLetterParser =
    from letter1 in Parsers.Letter
    from letter2 in Parsers.Char(letter1)
    from letter3 in Parsers.Char(letter1)
    select new string(letter1, 3);
```

The remaining two clauses we can use with parsers are the *let* clause and the *where* clause.
The let clause allows us to define additional variables.
If we reuse a piece of code in the query, we should to introduce a let clause to reduce the verbosity and unify the used values.
The where clause is used to check assumptions.
We provide a predicate to test; if the predicate fails, the entire created parser fails.

```csharp
Parser<char, char> parser =
    from c in Parsers.Letter
    where c != 'z' && c != 'Z'
    let laterChar = c + 1
    from _ in Parsers.Char(laterChar)
    select laterChar;
```

However, we recommend the `Assert` parser combinator instead of the where clause.
The where clause upon failure supplies only a generic message.
The `Assert` combinator is much more customizable.

[↩](#navigation)

#### Then and FollowedBy

The query syntax is a helpful tool.
However, the query implementation includes the creation of many lambda functions.
This is something that can significantly decrease performance.
As a piece of general advice for increased performance, try to avoid the query syntax unless necessary.
For this reason, ParsecCore also includes methods that more efficiently implement some common patterns of combining parsers.

First is the `Then` method.
This method applies the first parser, discards its result, applies the second parser, and returns its result.
If any of the two parsers fail, the parser created by the `Then` method also fails.
The two parsers in the next example are equivalent.

```csharp
Parser<char, char> thenParser = Parsers.Char('a').Then(Parsers.Char('b'));
Parser<char, char> queryParser =
    from _ in Parsers.Char('a')
    from b in Parsers.Char('b')
    select b;
```

The second is the `FollowedBy` method.
This method is very similar to the `Then` method.
The difference is that in this method, we discard the result of the \textit{second} parser and return the result of the \textit{first}.
The two parsers in the following example are again equivalent.

```csharp
Parser<char, char> thenParser = Parsers.Char('a').FollowedBy(Parsers.Char('b'));
Parser<char, char> queryParser =
    from a in Parsers.Char('a')
    from _ in Parsers.Char('b')
    select a;
```

[↩](#navigation)

#### All and String

We looked at one way of sequencing parsers.
Sometimes, however, we have a variable number of parsers to sequence.
Alternatively, sequencing these parsers in a query would be a chore.
In such an instance, we should use the `All` parser combinator.

The `All` parser combinator takes an enumerable of parsers and sequences them all, one after another.
If any of the component parsers fail, then the entire parser fails.
A list of their results is returned upon successful parsing of all constituent parsers.

A particular case of the `All` combinator is the `String` combinator.
`String`, as we can deduce from its name, parses a given string, a sequence of characters.
This combinator is one of the most used.
The parser of the string `"abc"`, the first example we used query syntax with, can thus be written in the following way.

```csharp
Parser<string, char> parserABC = Parsers.String("abc");
```

[↩](#navigation)

### Choice

Often, multiple different values are possible to parse.
For example, when parsing a boolean literal, there are two possibilities.
Either it is a string `true` or the string `false`.

We use the parser combinator `Or` in such situations.
This combinator composes two parsers.
It tries to parse the first parser.
If it succeeds, then it returns its result.
Otherwise, if it has not consumed any input, the second parser is tried, and its result is returned.
The issues around consuming input will be explained in the next section (see~\ref{chap:document:core:lookahead}).
If both errors fail, then their errors are combined.
This process of combining errors will also be explained in a later section (see~\ref{chap:document:core:errors}).

If we return to our example of parsing the boolean literals, the parser could look like this.

```csharp
Parser<bool, char> parserTrue =
    from t in Parsers.String("true")
    select true;
Parser<bool, char> parserTrue =
    from f in Parsers.String("false")
    select false;
Parser<bool, char> parserBool = parserTrue.Or(parserFalse);
```

When we want to decide between many such choices, we have two options.
We could chain many `Or` combinators together.
This, however, is needlessly verbose.
The other option is to use the `Choice` combinator.
This combinator takes a sequence of parsers, and the created parser returns the result of the first successful.
The following two parsers are semantically equivalent.

```csharp
// Definition of parsers a, b, and c

var parserOr = a.Or(b).Or(c);
var parserChoice = Parsers.Choice(a, b, c);
```

Since we are trying the parsers in a fixed sequence, we recommend putting the most likely option first.
In some instances, it can significantly speed up parsing.

[↩](#navigation)

### Lookahead

Some grammars require us to look many symbols ahead when choosing which alternative to proceed with.
Looking forward by multiple symbols at a time is called *lookahead*.
However, this can be an expensive operation.
Thus ParsecCore, by default, only looks forward by one character.

As stated previously, the `Or` combinator fails if the first parser does not succeed.
This is consistent with ParsecCore only looking forward by one character as the character parsers, `Char` and `Satisfy`, only consume input if they succeed.
However, we can change this behavior.
The `Try` combinator makes it so that if a parser fails, it does not consume any input.
In other words, the combinator *backtracks* in the parser input to where it was located at the start of the parse.
Afterward, we can use the resulting parser in the `Or` combinator.
Thus, we can choose which parser to go with based on more than one symbol.

Let us show how the choice and lookahead combinators work together.
Imagine we are parsing a language that has both `for` and `foreach` statements.
If we did not use the lookahead, `Try`, combinator, then we would not be able to parse some of the possible parser inputs correctly.

```csharp
IParserInput<char> input = Input.Create("foreach");

var parserFor = Parsers.String("for");
var parserForeach = Parsers.String("foreach");
// We first try to parse "for", then "foreach"
var parserForStmt = parserFor.Or(parserForeach);
// The parsing fails because we first try "for",
// this fails but consumes input,
// therefore the entire parser fails
var result = parserForStmt(input);
```

We need to introduce lookahead, in the form of the `Try` combinator, to look forward and decide which parser to choose.

```csharp
// Definitions of input, parserFor, parserForEach

// Notice the 'Try()'
var parserForStmt = parserFor.Try().Or(parserForeach);
// Parsing succeeds and outputs "foreach"
var result = parserForStmt(input);
```

There are more versions of lookahead beside `Try`.
The others are `LookAhead` and `NotFollowedBy`.
`LookAhead` backtracks in case the parser succeeds.
`NotFollowedBy` backtracks and reverses the result to a failure in the case the parser succeeds.

By default, if a parser fails while consuming input, then whichever combinator it is contained within fails as well.

[↩](#navigation)

### Errors

We shall now discuss the error system of ParsecCore.
When a parser fails, then in most situations, it means that the current symbol is different than we expected.
A default *standard error* is generated in such a scenario.
This error contains the unexpected symbol we have encountered and a message explaining the symbol we expected instead.

Another type of error is the *custom error*.
It represents situations other than the parser encountering an unexpected character.

When multiple errors are generated, such as when all parsers in the *Choice* combinator fail, they are combined.
They are combined based on which error is more specific.
An error is said to be more specific when it occurrs later in the parser input.
A secondary consideration is the input type, with the custom errors treated as more specific than the standard errors.

We, as users, can change the error handling somewhat.
First, we can label complex parsers with a more apt description.
This influences the message explaining what the parser expected.
The labeling is done by the *FailWith* combinator.

```csharp
var parserTrue =
    from t in Parsers.String("true")
    select true;
var parserTrue =
    from f in Parsers.String("false")
    select false;
// In the case of error the parser will output
// "boolean literal" as the expected value
// instead of a single character
var parserBool = parserTrue.Or(parserFalse).FailWith("boolean literal");
```

Second, we can also create a parser that outputs a particular error.
This is done by the `ParseError` and `Fail` parsers.

[↩](#navigation)

### Maybe

`Maybe` is a struct that is the output of some of the parsers.
It is a wrapper above a value that represents its possible nonexistence.
The struct can be in two states, either empty or containing a value.

We can ask about its state using the properties `IsEmpty` and `HasValue`.
If it contains a value, we can access it using the `Value` field or various extension methods such as `Else`, `Map`, and `Match`.

The `Maybe` value is created using two functions.
These are `Maybe.Nothing` and `Maybe.FromValue`.

[↩](#navigation)

### Other combinators

We would like to quickly mention other combinators.
We will not go into any great detail.
Nevertheless, we think it is useful for the user to know about different possibilities.
The details can be looked up later on.

First, we will mention the optional combinators `Optional` and `Option`.
These combinators successfully parse text, or they provide a default value.
Next, the chain combinators, such as `ChainL`, are convenient when parsing a left recursive grammar.
Afterward, we would like to mention the repetition combinators such as `Many`.
This combinator applies a single parser as many times as possible and returns a list of all the results.
We want to mention the many kinds of separator combinators which are similar to repetition combinators.
However, between each repetition, a separator parser is applied as well.
These combinators are helpful when we are parsing lists.

[↩](#navigation)

#### Indirect

Lastly, we would like to explain the `Indirect` parser combinator in detail.
The indirect parser combinator is used when there is a circular dependency between parsers.
For example, arithmetic grammars often have the following structure.

```backus-naur-form
    E -> E + E | E - E | F 
    F -> F * F | F / F | (E) | int
```

It is the `(E)` production that we are concerned with.
In C#, most ways we can initialize our parsers are sequential.
What are we supposed to do when two parsers are circularly dependent on each other?
We use the `Indirect` combinator.

The idea is fairly simple.
We wrap the first parser in a function, usually a lambda expression, and when it is time to use the parser, we invoke this function.
We capture the value of the second parser in this function.
Thus it is only when this function is called that the value of this parser is bound, and by that time, the second parser should be initialized.
And so we have solved the circular dependency.
The above grammar translated into ParsecCore would look similar to this code snippet (Please ignore that we would need to deal with the left recursion somehow).

```csharp
var parserExpr = Parsers.Choice(
    addParser,
    subParser,
    // Notice this parser and how we
    // wrap parserFactor inside a lambda expression
    Parsers.Indirect(() => parserFactor)
);

var parserFactor = Parsers.Choice(
    multiplyParser,
    divideParser,
    Parenthesized(parserExpr),
    integerParser
);
```

[↩](#navigation)

## Extensions

We will now turn to extensions that the library provides.
These tackle specific areas of parsing and make these areas more user-friendly.
For example, creating a parser of a particular permutation is possible.
However, we will supply a module that simplifies implementing such a parser.

[↩](#navigation)

### Expressions

One type of parser that is created relatively often is a parser for arithmetic expressions.
The \emph{Expressions} module, located in the `ParsecCore.Expressions` namespace, helps with writing this parser.

The Expressions module supports binary infix operators --- such as multiplication or division --- as well as prefix and postfix unary operators.
Operators are defined by one of three classes `PrefixUnary`, `PostfixUnary`, and `InfixBinary`.
We, as users, supply them with parsers that parse the given operator and output a function transforming the operands.
Additionally, the binary operators can decide their associativity.

The expression parser is created by supplying a two-dimensional array of operators.
The order of the arrays defines the priorities of the operators.
The earlier rows of operators have a higher priority than the later ones.
Along with the operators, we must also give a parser for the basic term.
This is the parser for integer literals, for instance.

The final arithmetic parser also detects whether the specified grammar is ambiguous.
This is, however, done only during runtime when parsing expressions.
During the parse, we detect if the operators are used ambiguously and return an error in such a case.

The arithmetic parser is created using the `Expression.Build` static method.
This function takes the two-dimensional array directly, or an `OperatorTable` can be defined that acts as a wrapper around this array.

As an example, we will show the definition of an arithmetic parser that includes the standard operators of a unary plus and minus, addition, subtraction, multiplication, and division.
This parser will work only with integers.
The provided example also evaluates the expression as it parses it.
Remaking the parser to provide only an abstract syntax tree of the expression is fairly trivial.

```csharp
var operatorTable = OperatorTable<int, char>.Create(
    new Operator<int, char>[][]
{
    new Operator<int, char>[]
    { // Highest priority - unary operators
        Expression.PrefixOperator<int>("+", x => x),
        Expression.PrefixOperator<int>("-", x => -x)
    },
    new Operator<int, char>[]
    { // Higher priority binary operators
        Expression.BinaryOperator<int>(
            "*", (x, y) => x * y, Associativity.Left
        ),
        Expression.BinaryOperator<int>(
            "/", (x, y) => x / y, Associativity.Left
        )
    },
    new Operator<int, char>[]
    {  // Lower priority binary operators
        Expression.BinaryOperator<int>(
            "+",(x, y) => x + y, Associativity.Left
        ),
        Expression.BinaryOperator<int>(
            "-",(x, y) => x - y, Associativity.Left
        )
    }
}
);
// Parser for natural numbers -- negative numbers 
// are handled with the unary operator '-'
// We could also add parenthesized expressions 
// using the Indirect combinator 
var termParser = Parsers.Natural.FollowedBy(Parsers.Spaces);

var expressionParser = Expression.Build(operatorTable, termParser);
```

Going further, we need only the `expressionParser` to parse any arithmetic expression.
The static methods `PrefixOperator` and `BinaryOperator` help with a quicker definition of operators.
These functions parse the provided string and any whitespace after them as the operator and return the given lambda function.

[↩](#navigation)

### Permutations

A permutation phrase is a sequence of elements where each element occurs exactly once, and the order is irrelevant.
Such phrases describe, for instance, XML tags or command-line arguments.
Additionally, some of the elements can be optional.

We will call the parsers of such phrases *permutation parsers*.
We will create permutation parsers using the `PermutationParser` class.
We can think of this class as a builder class for the final permutation parser.
New `PermutationParser` are created using the `Permutation.NewPermutation` and `Permutation.NewPermutationOptional` static methods.
These methods define the first element of the permutation to add.

More elements are added using the `Add` and `AddOptional` methods.
When adding new parsers, we not only add the parser itself but also add a function that tells the permutation parser how to combine the parsed element with the result of the rest of the permutation.
From now on, we will call this function the \emph{combining function}.
When a phrase is parsed, the combining functions are called one after the other, accumulating the parsing results.
We also need to provide a default value when we add parsers for optional elements.
The combining functions will use these values if the element is not present.
Finally, we create the permutation parser using the `GetParser` method.

Let us present an example of how to create a permutation parser.
We will show how to create a permutation parser for the elements 'a', 'b', 'c', and 'd'.
The final parser will therefore accept strings "abcd", "abdc", "acbd", and so on.

```csharp
var permutationParser =
    Permutation.NewPermutation(Parsers.Char('a'))
    .Add( // Add parser for element 'b'
        Parsers.Char('b'),
        // Create a tuple pair from 'a' and 'b'
        (a, b) => (a, b)
    ).Add( // Add parser for element 'c'
        Parsers.Char('c'),
        // Create a triple from 
        // the previously created pair and 'c'
        (pair, c) => (pair.Item1, pair.Item2, c)
    ).Add( // Add parser for element 'd'
    Parsers.Char('d'),
    // Create a quadruble
    (triple, d) => 
        (triple.Item1, triple.Item2, triple.Item3, d)
    ).GetParser();
```

We can see the combining functions accumulating the values of the parse.
In this case, we are creating larger and larger tuples.

[↩](#navigation)

### Indentation

Some programming languages make use of indentation as part of their syntax.
Examples of such languages are Python and Haskell.
The namespace `ParsecCore.Indentation` contains combinators that make parsing common indentation-sensitive constructs much easier.

The parsers `Indentation.Level` and `Indentation.Guard` are the foundation of the module.
Every other combinator in the module is constructed from these two parsers.
The parser `Level` is simple; it returns the current level of indentation.
The parser `Guard` accepts a relation --- greater than, greater than or equal, or equal --- and a reference indentation level.
Afterward, it tells us whether the current level of indentation satisfies the given relation when compared to the reference indentation level.

With only these parsers, the parsing of a variety of indentation-sensitive grammars is possible.
Nevertheless, these are relatively low-level parsers, and implementing a complex grammar would be cumbersome.
Thus the Indentation module provides combinators for specific common patterns.

[↩](#navigation)

#### NonIndented

The first of these is the `Indentation.NonIndented` combinator.
This is the simplest combinator.
It only checks that the provided element parsed by the provided parser is not indented.

```csharp
// The character 'a' has to be located
// at the beggining of a line
var parser = Indentation.NonIndented(
    Parsers.Spaces, Parsers.Char('a')
);
```

[↩](#navigation)

#### BlockMany

The next combinators we will discuss are the `Indentation.BlockMany` and the `Indentation.BlockMany1`.
These combinators serve to parse a statement block that is defined by whitespace.
Examples of such a statement block are Python statements.
The combinators parse the "head" of the statement and, afterward, the "body", a list of statements that occur at the same indentation.
For illustration, in a Python if statement, the head would be the keyword `if` a condition and colon followed by the end of the line.

```csharp
// Define parsers Statement, Expression, Keyword...

Parser<Expr, char> IfHead =
    from _ in Keyword("if")
    from condition in Expression
    from _ Colon
    select condition

Parser<IfStmt, char> If = Indentation.IndentationBlockMany1(
    PythonWhitespace,
    IfHead, // First parse the head of if statement
    Statement, // Then a list of indented statements
    (head, stmts) => new IfStmt(head, stmts)
);
```

The first item gives the level of indentation in the block.
If an item is indented differently than required, then an `IndentationError` is returned.
`IndentationError` is one of the items possible in a `CustomError`.
If the block of statements ends, i.e., the indentation is less than or equal to the indentation of the head item, then the parser stops.

```py
if a > 0: # The head item
    # The next three statements are part of the block
    foo() 
    bar()
    foobar()
another_statement() # Parser stops before this statement
```

The difference between `BlockMany` and `BlockMany1` is whether there has to be at least one item in the indented block.
In `BlockMany`, there does not have to be; the indented block can be empty.
In `BlockMany1`, at least one item has to be present.
Otherwise, it is an error.

[↩](#navigation)

#### Many

Further combinators are the `Indentation.Many` and `Indentation.Many1`.
These combinators serve to parse a list of items that satisfy a certain relation compared to a reference level of indentation.
For instance, if we want to parse a list of items all indented to column four, we would use the following combinator.

```csharp
var listParser = 
    Indentation.Many(
    (IndentationLevel)4, // Indent to column 4
    Relation.EQ // Items should have exactly indentation level of 4
    Parsers.Spaces,
    itemParser // How to parse items
);
```

The parser of the items is applied as many times as it succeeds.
The parser correctly ends when the list of items ends.
However, the item parser cannot consume any input while incorrectly attempting to parse the next statement.
Otherwise, the entire parser fails.

[↩](#navigation)

#### Optional

The following combinator is used if we need to parse an item while checking indentation, but the item may not be present.
In such a case, we use the `Indentation.Optional` combinator.
The created parser will attempt to parse the item.
It succeeds if the item is present and has the correct indentation.
Alternatively, it succeeds if the item is absent and the item parser does not consume any input.

[↩](#navigation)

#### Linefold

The final combinator that is provided is the `Indentation.LineFold` combinator.
\emph{Line-folding} appears when a statement can be spread across multiple lines.
Let us imagine we have a simple language with line-folding.
This language is made of keys and lists of values.
The values are separated by whitespace.
In such a case, the following two statements are equivalent.

```
Key: value1 value2 value3

Key: value1 value2
  value3
```

The second statement uses line-folding.
The statement continues until we parse an identifier that starts on indentation that is less than or equal to the indentation of the `Key`.
We can use the `LineFold` combinator to parse this language.

```csharp
// Definitions of parsers Id (Identifier),
// keyP (Key) and Spaces

var KeyValueParser = Indentation.LineFold(
    Spaces, // Whitespace to consume inbetween lines
    spaceConsumer => // Indentation checking parser
    {
        var valuesP =
            from values in Parsers.SepBy1(Id, spaceConsumer)
            from _ in Spaces
            select values;
        return from key in keyP
               from values in valuesP
               select (key, values);
    }
)
```

The `LineFold` combinator receives a function in which we defined the line-folding combinator.
This function has as a parameter a \emph{space consumer} parser.
We call it a space consumer because it usually parses an arbitrary amount of whitespace.
We define what characters this parser consumes using the first argument of the `LineFold` combinator.
This space consumer checks whether we are at the end of the linefold.
In the case we are, it fails without consuming input.
We use this argument to parse the list of values using the `valuesP` parser.
We subsequently also parse whitespace until the next key.

[↩](#navigation)

## Advice for writing parsers

During the description of our library, we stated several recommendations.
We want to add a few more.
Most of these concern the performance of the parsers.

[↩](#navigation)

### Create parsers only once

The first advice is to create parsers only once, whenever possible.
The creation of parsers can be expensive, and excessive instantiation of new parsers can hinder performance.
We recommend creating the parser as either a static field or as a variable that is reused whenever we need to parse any input.
One unfortunate aspect of the query syntax is that any parsers created inside the query are instantiated every time the query is invoked.

```csharp
var parser =
    from firstStr in Parsers.String("abc")
    from secondStr in Parsers.String("xyz")
    select firstStr + secondStr;
```

Here, both of the used parsers are newly instantiated whenever the query is invoked.
If we need to speed up our parsing, then it is worth considering moving the creation of the string parsers elsewhere and only referencing them from this parser.

[↩](#navigation)

### Avoid backtracking

The second piece of advice is to avoid backtracking if possible.
We use lookahead and backtracking if we cannot decide which path in parsing to choose based solely on the next character.
However, often we can eliminate this need by *left-factoring* our parser.
Left-factoring is the process of combining the common prefixes of the various paths.
Afterward, we can parse the prefix and only then decide which path to take using only the next symbol.
We thus transformed a parser with lookahead into a **LL(1)** parser.

An example of a parser where left-factoring is useful is the parser for escaped sequences in strings.
Often, we can either use hexadecimal digits to specify an exact symbol, or we can use a single character that acts as a shortcut.
For instance, `\x000A` and `\n` refer to the same character in different ways.
One possible way to parse these two different styles of escape sequences is to first attempt the single character option and, if unsuccessful, then the hexadecimal option.
There is, however, the problem that they both share the '\' character.
We would therefore need lookahead.
Consequently, the better way to parse them is first to parse the '\' and then attempt to parse either one of the shortcut characters or the character 'x'.

### Parse whitespace after each element

The last piece of advice we will state does not relate primarily to performance but to the ease of writing parsers themselves.
If we are parsing a text with whitespace and the whitespace is not significant, then we recommend parsing and discarding this whitespace after each element.
There are two benefits to this approach.
First, if we do not have a unified strategy for consuming whitespace, then the writing of the parse becomes more complicated.
This is because we have to keep in mind which parsers consume whitespace and which do not.
The second reason is that we do not want to have whitespace as a prefix in parsers.
In such a situation, we would have to constantly left-factor parsers; otherwise, we would need to use lookahead unnecessarily.

[↩](#navigation)
