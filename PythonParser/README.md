# Python Parser

This is a simple parser meant to illustrate the ability of ParsecCore to parse indentation sensitive grammars.
It is a parser of a simplified Python based on the version [3.0](https://docs.python.org/3.0/reference/index.html)
of Python. Version 3.0 is chosen since the grammar is the simplest and later versions add additional complexity to the
grammar outside of our example of parsing indentation.

Even then the grammar is still fairly complex and many of its features are quite minor and unimportant when it comes
to showcasing our ability to parse indentation. These are the features missing compared to the actual grammar of 
Python 3.0:

- Encoding declaration
- **Literals**
  - Long strings
  - Bytes literals
  - Escape sequences outside of 8 bit and 16 bit hexadecimal sequences
  - Imaginary and complex numbers
- **Expressions**
  - Comprehensions and generator expressions
  - Anything to do with the `yield` keyword
  - Keyword arguments in calls
  - Bitwise operators
  - Conditional expression
  - Lambdas
- **Statements**
  - Simple statements
    - Multiple assignments on the same line (chaining of the `=` operator)
    - Augmented assignment operations
    - `assert`
    - `del`
    - `yield`
    - `raise`
    - `future`
    - `global` and `nonlocal` keywords
  Compound statements
    - `try`
    - `with`
    - Default parameters in function definitions
    - Class definitions

While the list is large the language that remains is still perfectly capable of showcasing our ability of parsing
indentations. 