using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Extensions
{
    partial class SyntaxExtensions
    {
        /// <summary>
        /// Returns a new <see cref="MethodDeclarationSyntax"/> with
        /// a new body, and no <see langword="extern"/> keyword.
        /// </summary>
        public static MethodDeclarationSyntax NotExtern(this MethodDeclarationSyntax method, CSharpSyntaxNode body)
        {
            method = method.WithModifiers(method.Modifiers.Remove(method.Modifiers.First(x => x.IsKind(SyntaxKind.ExternKeyword))));

            if (body is BlockSyntax block)
                return method.WithBody(block);
            if (body is StatementSyntax stmt)
                return method.WithBody(SyntaxFactory.Block(stmt));
            if (body is ExpressionSyntax expr)
                return method.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expr));
            if (body is ArrowExpressionClauseSyntax arrow)
                return method.WithExpressionBody(arrow);

            throw new ArgumentException("Invalid body node.", nameof(body));
        }

        /// <summary>
        /// Returns the given <see cref="StatementSyntax"/> with an added semicolon.
        /// </summary>
        public static T WithSemicolon<T>(this T stmt) where T : StatementSyntax
        {
            SyntaxToken semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            switch ((object)stmt)
            {
                case ExpressionStatementSyntax estmt:
                    return estmt.WithSemicolonToken(semicolon) as T;
                case ReturnStatementSyntax rstmt:
                    return rstmt.WithSemicolonToken(semicolon) as T;
                case YieldStatementSyntax ystmt:
                    return ystmt.WithSemicolonToken(semicolon) as T;
                case ThrowStatementSyntax tstmt:
                    return tstmt.WithSemicolonToken(semicolon) as T;
                case ContinueStatementSyntax cstmt:
                    return cstmt.WithSemicolonToken(semicolon) as T;
                case BreakStatementSyntax bstmt:
                    return bstmt.WithSemicolonToken(semicolon) as T;
                case GotoStatementSyntax gstmt:
                    return gstmt.WithSemicolonToken(semicolon) as T;
                case EmptyStatementSyntax estmt:
                    return estmt.WithSemicolonToken(semicolon) as T;
                case LocalDeclarationStatementSyntax lstmt:
                    return lstmt.WithSemicolonToken(semicolon) as T;

                default:
                    return stmt;
            }
        }

        /// <summary>
        /// Returns the given <see cref="StatementSyntax"/> with a different expression.
        /// </summary>
        public static T WithExpression<T>(this T stmt, ExpressionSyntax expr, bool throwIfNotFound = false) where T : StatementSyntax
        {
            switch ((object)stmt)
            {
                case ExpressionStatementSyntax estmt:
                    return estmt.WithExpression(expr) as T;
                case ReturnStatementSyntax rstmt:
                    return rstmt.WithExpression(expr) as T;
                case YieldStatementSyntax ystmt:
                    return ystmt.WithExpression(expr) as T;
                case ThrowStatementSyntax tstmt:
                    return tstmt.WithExpression(expr) as T;
                case UsingStatementSyntax ustmt:
                    return ustmt.WithExpression(expr) as T;
                case SwitchStatementSyntax sstmt:
                    return sstmt.WithExpression(expr) as T;
                case IfStatementSyntax istmt:
                    return istmt.WithCondition(expr) as T;
                case LockStatementSyntax lstmt:
                    return lstmt.WithExpression(expr) as T;
                case ForEachStatementSyntax fstmt:
                    return fstmt.WithExpression(expr) as T;
                case WhileStatementSyntax wstmt:
                    return wstmt.WithCondition(expr) as T;
                case DoStatementSyntax dstmt:
                    return dstmt.WithCondition(expr) as T;

                default:
                    if (throwIfNotFound)
                        throw new ProcessingException(stmt, "The given statement does not accept an expression.");
                    else
                        return stmt;
            }
        }

        /// <summary>
        /// Returns the expression of the given statement.
        /// </summary>
        public static ExpressionSyntax Expression<T>(this T stmt) where T : StatementSyntax
        {
            switch ((object)stmt)
            {
                case ExpressionStatementSyntax estmt:
                    return estmt.Expression;
                case ReturnStatementSyntax rstmt:
                    return rstmt.Expression;
                case YieldStatementSyntax ystmt:
                    return ystmt.Expression;
                case ThrowStatementSyntax tstmt:
                    return tstmt.Expression;
                case UsingStatementSyntax ustmt:
                    return ustmt.Expression;
                case SwitchStatementSyntax sstmt:
                    return sstmt.Expression;
                case IfStatementSyntax istmt:
                    return istmt.Condition;
                case LockStatementSyntax lstmt:
                    return lstmt.Expression;
                case ForEachStatementSyntax fstmt:
                    return fstmt.Expression;
                case WhileStatementSyntax wstmt:
                    return wstmt.Condition;
                case DoStatementSyntax dstmt:
                    return dstmt.Condition;

                default:
                    return null;
            }
        }

        internal static SwitchStatementSyntax MemberSwitch(ExpressionSyntax memberExpr, Func<ExpressionSyntax, ExpressionSyntax> expr)
        {
            return $@"
switch ({memberExpr})
{{
    case PropertyDeclarationSyntax prop:
        return {expr(SyntaxFactory.IdentifierName("prop"))};
    case FieldDeclarationSyntax field:
        return {expr(SyntaxFactory.IdentifierName("field"))};
    case TypeDeclarationSyntax type:
        return {expr(SyntaxFactory.IdentifierName("type"))};
    case DelegateDeclarationSyntax del:
        return {expr(SyntaxFactory.IdentifierName("del"))};
    case EnumDeclarationSyntax @enum:
        return {expr(SyntaxFactory.IdentifierName("@enum"))};
    case MethodDeclarationSyntax method:
        return {expr(SyntaxFactory.IdentifierName("method"))};
    case EventFieldDeclarationSyntax eventfld:
        return {expr(SyntaxFactory.IdentifierName("eventfld"))};
    case EventDeclarationSyntax @event:
        return {expr(SyntaxFactory.IdentifierName("@event"))};
    case ConstructorDeclarationSyntax ctor:
        return {expr(SyntaxFactory.IdentifierName("ctor"))};
    case DestructorDeclarationSyntax dtor:
        return {expr(SyntaxFactory.IdentifierName("dtor"))};
    case IndexerDeclarationSyntax index:
        return {expr(SyntaxFactory.IdentifierName("index"))};
    case OperatorDeclarationSyntax op:
        return {expr(SyntaxFactory.IdentifierName("op"))};
    case ConversionOperatorDeclarationSyntax convOp:
        return {expr(SyntaxFactory.IdentifierName("convOp"))};
    case IncompleteMemberSyntax incomplete:
        return {expr(SyntaxFactory.IdentifierName("incomplete"))};
    default:
        throw new ArgumentException(""The given argument was not a member."", ""{memberExpr}"");
}}".Syntax<SwitchStatementSyntax>();
        }
    }
}
