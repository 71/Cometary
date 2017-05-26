using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Rewriting
{
    #region Quote
    /// <summary>
    /// Represents a <see langword="void"/> quote, used by templates.
    /// </summary>
    public class Quote
    {
        /// <summary>
        /// Gets the syntax of the invocation of the template.
        /// </summary>
        public InvocationExpressionSyntax Syntax { get; }

        /// <summary>
        /// Gets the symbol of the invocation of the template.
        /// </summary>
        public IInvocationExpression Symbol { get; }

        /// <summary>
        /// Gets the 0-based index of the quoted argument in the invocation.
        /// </summary>
        public int Sequence { get; }


        /// <summary>
        /// Gets the syntax of the quoted argument.
        /// </summary>
        public ArgumentSyntax ArgumentSyntax => Syntax.ArgumentList.Arguments[Sequence];

        /// <summary>
        /// Gets the symbol of the quoted argument.
        /// </summary>
        public IArgument ArgumentSymbol => Symbol.ArgumentsInParameterOrder[Sequence];

        /// <summary>
        /// Gets whether or not the invocation was made in an anonymous function.
        /// </summary>
        public bool IsAnonymous { get; }

        /// <summary>
        /// Gets whether or not the invocation was made in a property accessor.
        /// </summary>
        public bool IsProperty { get; }

        /// <summary>
        /// Gets the syntax of the method in which the invocation was made.
        /// </summary>
        public BaseMethodDeclarationSyntax MethodSyntax { get; }

        /// <summary>
        /// Gets the syntax of the anonymous method in which the invocation was made.
        /// </summary>
        public AnonymousFunctionExpressionSyntax AnonymousMethodSyntax { get; }

        /// <summary>
        /// Gets the syntax of the accessor in which the invocation was made.
        /// </summary>
        public AccessorDeclarationSyntax AccessorSyntax { get; }

        /// <summary>
        /// Gets the syntax of the property in which the invocation was made.
        /// </summary>
        public BasePropertyDeclarationSyntax PropertySyntax { get; }

        /// <summary>
        /// Gets the symbol of the method in which the invocation was made.
        /// </summary>
        public IMethodSymbol MethodSymbol { get; }

        /// <summary>
        /// Gets the symbol of the anonymous method in which the invocation was made.
        /// </summary>
        public ILambdaExpression AnonymousMethodSymbol { get; }

        /// <summary>
        /// Gets the symbol of the property in which the invocation was made.
        /// </summary>
        public IPropertySymbol PropertySymbol { get; }

        /// <summary>
        /// Gets the syntax of the body in which the invocation was made.
        /// </summary>
        public CSharpSyntaxNode BodySyntax { get; }

        /// <summary>
        /// Gets the symbol of the body in which the invocation was made.
        /// </summary>
        public IBlockStatement BodySymbol { get; }

        /// <summary>
        /// Gets the <see cref="Unquote(SyntaxNode)"/> produced by this
        /// node.
        /// </summary>
        internal Unquote Result { get; private set; }

        private readonly Quote _ref;

        internal Quote(InvocationExpressionSyntax syntax, IInvocationExpression symbol)
        {
            Syntax = syntax;
            Symbol = symbol;

            // Check if we're in an anonymous method.
            var anon = syntax.FirstAncestorOrSelf<AnonymousFunctionExpressionSyntax>();

            if (anon != null)
            {
                IsAnonymous = true;
                AnonymousMethodSyntax = anon;
                AnonymousMethodSymbol = anon.SyntaxTree.Model().GetOperation(anon) as ILambdaExpression;

                BodySyntax = AnonymousMethodSyntax.Body;
                BodySymbol = AnonymousMethodSymbol?.Body;
            }

            // Check if we're in a property accessor / method.
            var accessor = syntax.FirstAncestorOrSelf<AccessorDeclarationSyntax>();

            if (accessor != null)
            {
                IsProperty = true;
                AccessorSyntax = accessor;
                PropertySyntax = (accessor.Parent as AccessorListSyntax)?.Parent as BasePropertyDeclarationSyntax;

                PropertySymbol = accessor.SyntaxTree.Model().GetDeclaredSymbol(PropertySyntax) as IPropertySymbol;
                MethodSymbol = AccessorSyntax.Keyword.Text == "get" ? PropertySymbol?.GetMethod : PropertySymbol?.SetMethod;

                if (!IsAnonymous)
                {
                    BodySyntax = (CSharpSyntaxNode)AccessorSyntax.Body ?? AccessorSyntax.ExpressionBody.Expression;
                    BodySymbol = BodySyntax.SyntaxTree.Model().GetOperation(BodySyntax) as IBlockStatement;
                }
            }
            else
            {
                MethodSyntax = syntax.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
                MethodSymbol = ModelExtensions.GetDeclaredSymbol(MethodSyntax.SyntaxTree.Model(), MethodSyntax) as IMethodSymbol;

                if (!IsAnonymous)
                {
                    BodySyntax = (CSharpSyntaxNode)MethodSyntax.Body ?? MethodSyntax.ExpressionBody.Expression;
                    BodySymbol = BodySyntax.SyntaxTree.Model().GetOperation(BodySyntax) as IBlockStatement;
                }
            }
        }

        [SuppressMessage("ReSharper", "AssignmentInConditionalExpression", Justification = "Yeah, I did it on purpose.")]
        internal Quote(Quote original, int sequence)
        {
            _ref = original;

            Syntax = original.Syntax;
            Symbol = original.Symbol;
            Sequence = sequence;

            BodySyntax = original.BodySyntax;
            BodySymbol = original.BodySymbol;

            if (IsAnonymous = original.IsAnonymous)
            {
                AnonymousMethodSyntax = original.AnonymousMethodSyntax;
                AnonymousMethodSymbol = original.AnonymousMethodSymbol;
            }

            if (IsProperty = original.IsProperty)
            {
                AccessorSyntax = original.AccessorSyntax;

                PropertySyntax = original.PropertySyntax;
                PropertySymbol = original.PropertySymbol;
            }
            else
            {
                MethodSyntax = original.MethodSyntax;
            }

            MethodSymbol = original.MethodSymbol;
        }

        /// <summary>
        /// Clones the current quote to another <see cref="Quote{T}"/>
        /// of the given target type.
        /// </summary>
        internal object For(int nth, Type targetType) => Activator.CreateInstance(typeof(Quote<>).MakeGenericType(targetType), this, nth);

        /// <summary>
        /// Clones the current quote to another <see cref="Quote"/>.
        /// </summary>
        internal object For(int nth) => new Quote(this, nth);

        /// <summary>
        /// Unquot
        /// </summary>
        public Unquote Unquote(SyntaxNode node)
        {
            if (_ref != null)
                return _ref.Unquote(node);

            return Result = new Unquote(node);
        }

        /// <summary>
        /// 
        /// </summary>
        public Unquote<T> Unquote<T>(SyntaxNode node)
        {
            if (_ref != null)
                return _ref.Unquote<T>(node);

            return (Result = new Unquote<T>(node)) as Unquote<T>;
        }
    }

    /// <summary>
    /// Represents a quote, used by templates.
    /// </summary>
    public sealed class Quote<T> : Quote
    {
        internal Quote(Quote quote, int nth) : base(quote, nth)
        {
        }

        /// <summary>
        /// Implicitely converts an expression of type T into a quote.
        /// </summary>
        public static implicit operator Quote<T>(T value)
        {
            throw new InvalidOperationException();
        }
    }
    #endregion

    #region Unquote
    /// <summary>
    /// Represents an abstract <see langword="void"/>
    /// return value, used in templates.
    /// <para>
    /// If a statement or expression is returned, the invocation
    /// will be replaced. If a method or method body is given,
    /// the method itself will be replaced.
    /// </para>
    /// </summary>
    public class Unquote
    {
        /// <summary>
        /// Gets the syntax of the produced value.
        /// </summary>
        public SyntaxNode Syntax { get; }

        /// <summary>
        /// Gets whether or not this unquote returns <see langword="void"/>.
        /// </summary>
        public virtual bool IsVoidQuote => true;

        internal Unquote(SyntaxNode node)
        {
            Syntax = node;
        }
    }

    /// <summary>
    /// Represents an abstract value, used during compilation to
    /// alter the return value of a template.
    /// </summary>
    public sealed class Unquote<T> : Unquote
    {
        /// <inheritdoc />
        public override bool IsVoidQuote => false;

        internal Unquote(SyntaxNode node) : base(node)
        {
        }

        /// <summary>
        /// Gets the compile-time value of the <paramref name="unquote"/>.
        /// </summary>
        public static implicit operator T(Unquote<T> unquote) => default(T);
    }
    #endregion
}
