# ParsecCore

ParsecCore is a parser combinator library inspired by [Parsec](https://hackage.haskell.org/package/parsec).
For a thorough tutorial for ParsecCore see [tutorial.md](tutorial.md)

## Example projects

ParsecCore comes with two example applications

- JSONtoXML
- PythonParser

These are provided to demonstrate a possible use of the library.
They both require .NET 6.0 or later.
The .NET sdk needed can be downloaded [here](https://dotnet.microsoft.com/en-us/download).

### JSONtoXML

*JSONtoXML* is a simple application that takes a JSON file as input and outputs an XML~\cite{xml} file with the equivalent contents.
The application is located in the directory `examples/src/JSONParser`.
If no output file is specified, the XML is emitted into standard output.
The input and output files are specified with the `--input` and `---output` command-line arguments, respectively.

The application can be invoked using the command `dotnet run` in the project directory.
There are also example JSON files present in the subdirectory `examples/src/JSONParser/example-json`.
The entire invocation can thus be for example:

```console
dotnet run -- --input example-json/sample1.json --output sample.xml
```

A new file sample.xml would be created that would have the same contents as sample1.json but in XML format.

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
    <fruit>
        Apple
    </fruit>
    <size>
        Large
    </size>
    <color>
        Red
    </color>
</root>
```

### PythonParser

*PythonParser* is a program that parses a simplified version of Python.
It takes a python source code as an input, parses it into internal structures, and then outputs it.
PythonParser is located in the directory `examples/src/PythonParser`.

The application can be invoked using the command `dotnet run` in the project directory.
There are example Python files present: `example.py` and `example-error.py`.
The entire invocation can thus be for example:

```console
dotnet run -- example.py
```

The file `example-error.py` has an incorrect indentation on line 8 and the parser reports it.

```console
> dotnet run -- .\example-error.py
  (8:3):
    Incorrect indentation (expected indentation equal to 4, encountered 2)
```

## Tests

The ParsecCore library and the example applications come with unit tests which can be invoked using

```console
dotnet test
```

in the top solution directory.
