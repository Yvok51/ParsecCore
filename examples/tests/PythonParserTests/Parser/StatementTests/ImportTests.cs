using ParsecCore.Input;
using ParsecCore.MaybeNS;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class ImportTests
    {
        [Fact]
        public void SimpleImport()
        {
            var input = ParserInput.Create("import numpy\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ImportModule(
                        new List<IdentifierLiteral>() 
                        {
                            new IdentifierLiteral("numpy")
                        },
                        Maybe.Nothing<IdentifierLiteral>()
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void LongerModulePath()
        {
            var input = ParserInput.Create("import numpy.typing\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ImportModule(
                        new List<IdentifierLiteral>()
                        {
                            new IdentifierLiteral("numpy"), new IdentifierLiteral("typing")
                        },
                        Maybe.Nothing<IdentifierLiteral>()
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void ModuleAlias()
        {
            var input = ParserInput.Create("import numpy as np\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ImportModule(
                        new List<IdentifierLiteral>()
                        {
                            new IdentifierLiteral("numpy")
                        },
                        Maybe.FromValue(new IdentifierLiteral("np"))
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void SpecificImport()
        {
            var input = ParserInput.Create("from numpy import Inf\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ImportSpecific(
                        new List<IdentifierLiteral>()
                        {
                            new IdentifierLiteral("numpy")
                        },
                        new IdentifierLiteral("Inf"),
                        Maybe.Nothing<IdentifierLiteral>()
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void SpecificImportLongerModulePath()
        {
            var input = ParserInput.Create("from numpy.typing import NBitBase\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ImportSpecific(
                        new List<IdentifierLiteral>()
                        {
                            new IdentifierLiteral("numpy"), new IdentifierLiteral("typing")
                        },
                        new IdentifierLiteral("NBitBase"),
                        Maybe.Nothing<IdentifierLiteral>()
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void SpecificImportAlias()
        {
            var input = ParserInput.Create("from numpy import Inf as NPInf\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ImportSpecific(
                        new List<IdentifierLiteral>()
                        {
                            new IdentifierLiteral("numpy")
                        },
                        new IdentifierLiteral("Inf"),
                        Maybe.FromValue(new IdentifierLiteral("NPInf"))
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void ImportAll()
        {
            var input = ParserInput.Create("from numpy import *\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new ImportSpecificAll(
                        new List<IdentifierLiteral>()
                        {
                            new IdentifierLiteral("numpy")
                        }
                    )
                }),
                result.Result
            );
        }
    }
}
