using ParsecCore.Indentation;
using PythonParser.Structures;
using System.Text;

namespace PythonParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }

        private struct Nothing { }

        private class PrintVisitor : ExprVisitor<string, int>, StmtVisitor<string, int>
        {
            private static string GetIndentation(int indentationLevel) => new string(' ', indentationLevel * 4);

            private string ListExprs(IReadOnlyList<Expr> exprs, int indent)
                => string.Join(", ", exprs.Select(expr => expr.Accept(this, indent)));

            private static Dictionary<BinaryOperator, string> BinaryOperators =
                new Dictionary<BinaryOperator, string>()
                {
                    { BinaryOperator.And, "and" },
                    { BinaryOperator.DoubleSlash, "//" },
                    { BinaryOperator.DoubleStar, "**" },
                    { BinaryOperator.Equal, "==" },
                    { BinaryOperator.GE, ">=" },
                    { BinaryOperator.GT, ">" },
                    { BinaryOperator.In, "in" },
                    { BinaryOperator.Is, "is" },
                    { BinaryOperator.IsNot, "is not" },
                    { BinaryOperator.LE, "<=" },
                    { BinaryOperator.LT, "<" },
                    { BinaryOperator.Minus, "-" },
                    { BinaryOperator.Modulo, "%" },
                    { BinaryOperator.NotEqual, "!=" },
                    { BinaryOperator.NotIn, "not in" },
                    { BinaryOperator.Or, "or" },
                    { BinaryOperator.Plus, "+" },
                    { BinaryOperator.Slash, "/" },
                    { BinaryOperator.Star, "*" },
                };

            private static Dictionary<UnaryOperator, string> UnaryOperators =
                new Dictionary<UnaryOperator, string>()
                {
                    { UnaryOperator.Minus, "-" },
                    { UnaryOperator.Plus, "+" },
                    { UnaryOperator.Not, "!" },
                };

            public string VisitAssignment(Assignment assignment, int indentation)
            {
                var builder = new StringBuilder();
                builder.Append(GetIndentation(indentation));
                foreach (var targets in assignment.TargetList)
                {
                    builder.Append(ListExprs(targets, indentation));
                    builder.Append(" = ");
                }
                builder.Append(ListExprs(assignment.Expressions, indentation));

                return builder.ToString();
            }

            public string VisitAtrributeRef(AttributeRef attributeRef, int indentation)
            {
                return attributeRef.Obj.Accept(this, indentation) + attributeRef.Attribute.Accept(this, indentation);
            }

            public string VisitBinary(Binary binary, int indentation)
            {
                return binary.Left.Accept(this, indentation) 
                    + ' ' + BinaryOperators[binary.Operator] 
                    + ' ' + binary.Right.Accept(this, indentation);
            }

            public string VisitBooleanLiteral(BooleanLiteral literal, int indentation)
            {
                return literal.Value ? "True" : "False";
            }

            public string VisitBreak(Break @break, int indentation)
            {
                return GetIndentation(indentation) + "break";
            }

            public string VisitCall(Call call, int indentation)
            {
                var builder = new StringBuilder();
                if (!call.ArgumentList.IsEmpty)
                {
                    builder.Append(ListExprs(call.ArgumentList.Value, indentation));
                }
                if (!call.KeywordArguments.IsEmpty)
                {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append(ListExprs(call.KeywordArguments.Value, indentation));
                }
                if (!call.SequenceExpr.IsEmpty)
                {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append('*');
                    builder.Append(call.SequenceExpr.Value.Accept(this, indentation));
                }
                if (!call.MappingExpr.IsEmpty)
                {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append("**");
                    builder.Append(call.MappingExpr.Value.Accept(this, indentation));
                }
                return call.CalledExpr.Accept(this, indentation) + '(' + builder.ToString() + ')';
            }

            public string VisitContinue(Continue @continue, int indentation)
            {
                return GetIndentation(indentation) + "continue";
            }

            public string VisitDictDisplay(DictDisplay keyDisplay, int indentation)
            {
                return '{' + ListExprs(keyDisplay.Data, indentation) + '}';
            }

            public string VisitExpression(ExpressionStmt expression, int indentation)
            {
                return GetIndentation(indentation) + expression.Accept(this, indentation);
            }

            public string VisitFloatLiteral(FloatLiteral literal, int indentation)
            {
                return literal.Value.ToString();
            }

            public string VisitFor(For forStatement, int indentation)
            {
                throw new NotImplementedException();
            }

            public string VisitFunction(Function function, int aindentationrg)
            {
                throw new NotImplementedException();
            }

            public string VisitIdentifierLiteral(IdentifierLiteral literal, int indentation)
            {
                return literal.Name;
            }

            public string VisitIf(If ifStatement, int indentation)
            {
                throw new NotImplementedException();
            }

            public string VisitImport(ImportModule import, int indentation)
            {
                return "import " 
                    + string.Join('.', import.ModulePath) 
                    + import.Alias.Match(just: id => " as " + id, nothing: () => string.Empty);
            }

            public string VisitImportSpecific(ImportSpecific import, int indentation)
            {
                return "from "
                    + string.Join('.', import.ModulePath)
                    + " import " + import.Specific
                    + import.Alias.Match(just: id => "as " + id, nothing: () => string.Empty);
            }

            public string VisitImportSpecificAll(ImportSpecificAll import, int indentation)
            {
                return "from " + string.Join('.', import.ModulePath) + " import *";
            }

            public string VisitIntegerLiteral(IntegerLiteral literal, int indentation)
            {
                return literal.Value.ToString();
            }

            public string VisitKeyDatum(KeyDatum keyDatum, int indentation)
            {
                return keyDatum.Key.Accept(this, indentation) + ": " + keyDatum.Value.Accept(this, indentation);
            }

            public string VisitKeywordArgument(KeywordArgument keywordArgument, int indentation)
            {
                return keywordArgument.Name.Accept(this, indentation) 
                    + " = "
                    + keywordArgument.Value.Accept(this, indentation);
            }

            public string VisitListDisplay(ListDisplay listDisplay, int indentation)
            {
                return '[' + ListExprs(listDisplay.Expressions, indentation) + ']';
            }

            public string VisitNoneLiteral(NoneLiteral literal, int indentation)
            {
                return "None";
            }

            public string VisitParenthForm(ParenthForm parenthForm, int indentation)
            {
                return '('
                    + ListExprs(parenthForm.Expressions, indentation)
                    + ')';
            }

            public string VisitPass(Pass pass, int indentation)
            {
                return GetIndentation(indentation) + "pass";
            }

            public string VisitReturn(Return @return, int indentation)
            {
                return GetIndentation(indentation)
                    + "return"
                    + @return.Expressions.Match(
                        just: exprs => ' ' + ListExprs(exprs, indentation),
                        nothing: () => string.Empty
                      );
            }

            public string VisitSetDisplay(SetDisplay setDisplay, int indentation)
            {
                return '{' + ListExprs(setDisplay.Expressions, indentation) + '}';
            }

            public string VisitSlice(Slice slice, int indentation)
            {
                return slice.Slicable.Accept(this, indentation) + '[' + ListExprs(slice.Slices, indentation) + ']';
            }

            public string VisitSliceItem(SliceItem sliceItem, int indentation)
            {
                return sliceItem.LowerBound.Match(just: expr => expr.Accept(this, indentation), nothing: () => string.Empty)
                    + ':' + sliceItem.UpperBound.Match(just: expr => expr.Accept(this, indentation), nothing: () => string.Empty)
                    + ':' + sliceItem.Stride.Match(just: expr => expr.Accept(this, indentation), nothing: () => string.Empty);
            }

            public string VisitStringLiteral(StringLiteral literal, int indentation)
            {
                return literal.Prefix + '"' + literal.Value + '"';
            }

            public string VisitSubscription(Subscription subscription, int indentation)
            {
                return subscription.Subscribable.Accept(this, indentation)
                    + '['
                    + ListExprs(subscription.Expressions, indentation)
                    + ']';
            }

            public string VisitSuite(Suite suite, int indentation)
            {
                var builder = new StringBuilder();
                var indent = GetIndentation(indentation);
                foreach (var stmt in suite.Statements)
                {
                    builder.Append(indent);
                    builder.AppendLine(stmt.Accept(this, indentation));
                }
                return builder.ToString();
            }

            public string VisitUnary(Unary unary, int indentation)
            {
                return UnaryOperators[unary.Operator] + unary.Expression.Accept(this, indentation);
            }

            public string VisitWhile(While ifStatement, int indentation)
            {
                throw new NotImplementedException();
            }
        }
    }
}