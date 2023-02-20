﻿using ParsecCore.Input;
using PythonParser.Parser;
using PythonParser.Structures;

namespace PythonParserTests.Parser.StatementTests
{
    public class AssignmentTests
    {
        [Fact]
        public void SimpleAssignment()
        {
            var input = ParserInput.Create("x = 4\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Assignment(
                        new List<List<Expr>>()
                        {
                            new List<Expr>() { new IdentifierLiteral("x") }
                        },
                        new List<Expr>() { new IntegerLiteral(4) }
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void SimpleAssignmentIdentifier()
        {
            var input = ParserInput.Create("x = y\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Assignment(
                        new List<List<Expr>>()
                        {
                            new List<Expr>() { new IdentifierLiteral("x") }
                        },
                        new List<Expr>() { new IdentifierLiteral("y") }
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void MultipleTargets()
        {
            var input = ParserInput.Create("x, y = pair\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Assignment(
                        new List<List<Expr>>()
                        {
                            new List<Expr>() { new IdentifierLiteral("x"), new IdentifierLiteral("y") }
                        },
                        new List<Expr>() { new IntegerLiteral(4) }
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void MultipleAssignments()
        {
            var input = ParserInput.Create("x = y = pair\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Assignment(
                        new List<List<Expr>>()
                        {
                            new List<Expr>() { new IdentifierLiteral("x") },
                            new List<Expr>() { new IdentifierLiteral("y") }
                        },
                        new List<Expr>() { new IntegerLiteral(4) }
                    )
                }),
                result.Result
            );
        }

        [Fact]
        public void MultipleAssignmentsMultipleTargets()
        {
            var input = ParserInput.Create("x, y = w, z = pair\n");
            var result = Statements.Statement(input);

            Assert.True(result.IsResult);
            Assert.Equal(
                new Suite(new List<Stmt>()
                {
                    new Assignment(
                        new List<List<Expr>>()
                        {
                            new List<Expr>() { new IdentifierLiteral("x"), new IdentifierLiteral("y") },
                            new List<Expr>() { new IdentifierLiteral("w"), new IdentifierLiteral("z") }
                        },
                        new List<Expr>() { new IntegerLiteral(4) }
                    )
                }),
                result.Result
            );
        }
    }
}
