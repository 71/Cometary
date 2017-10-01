using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary
{
    using Extensions;
    using System.Linq;

    /// <summary>
    /// Represents a <see langword="void"/> quote, used by macros
    /// to insert code dynamically in a method body.
    /// </summary>
    public partial class Quote
    {
        #region CleanUp registration
        static Quote()
        {
            CleanUp.ShouldDelete += ShouldDelete;

            bool IsQuoteParameter(ParameterSyntax parameter)
                => parameter.Type is SimpleNameSyntax name && name.Identifier.Text == nameof(Quote)
                || parameter.Type is GenericNameSyntax gen && gen.Identifier.Text == nameof(Quote);

            bool ShouldDelete(SyntaxNode node)
                => node is LocalFunctionStatementSyntax fun && fun.ParameterList.Parameters.Any(IsQuoteParameter)
                || node is MethodDeclarationSyntax method && method.ParameterList.Parameters.Any(IsQuoteParameter);
        }
        #endregion

        #region Properties
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
        /// Gets or sets the nodes inserted via this <see cref="Quote"/>.
        /// </summary>
        internal List<SyntaxNode> InsertedNodes { get; set; }
        #endregion

        #region Constructors
        internal Quote(InvocationExpressionSyntax syntax, IInvocationExpression symbol)
        {
            Syntax = syntax;
            Symbol = symbol;
            InsertedNodes = new List<SyntaxNode>();

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
            InsertedNodes = original.InsertedNodes;
        }
        #endregion

        #region Methods
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
        /// Returns a new identical <see cref="Quote"/>, with an additional
        /// inserted node.
        /// </summary>
        internal Quote Push(SyntaxNode node)
        {
            InsertedNodes.Add(node);

            return this;
        }

        /// <summary>
        /// Adds the given statement to the output.
        /// </summary>
        public void Add(StatementSyntax stmt) => Push(stmt);

        /// <summary>
        /// Adds the given expression to the output.
        /// </summary>
        public void Add(ExpressionSyntax expr) => Push(expr);

        /// <summary>
        /// Adds the given code to the output.
        /// </summary>
        public void Add(string code) => Push(code.Syntax<StatementSyntax>());
        #endregion

        #region Operators
        /// <summary>
        /// Adds the given statement to the output, and returns <see langword="this"/>.
        /// </summary>
        public static Quote operator +(Quote quote, StatementSyntax stmt) => quote.Push(stmt);

        /// <summary>
        /// Adds the given expression to the output as a statement, and returns <see langword="this"/>.
        /// </summary>
        public static Quote operator +(Quote quote, ExpressionSyntax expr) => quote.Push(expr);

        /// <summary>
        /// Writes the given code to the output, and returns <see langword="this"/>.
        /// </summary>
        public static Quote operator +(Quote quote, string code) => quote.Push(code.Syntax<StatementSyntax>());

        /// <summary>
        /// Returns the given expression as if it were a C# value.
        /// </summary>
        public static object operator >(Quote quote, ExpressionSyntax expr)
        {
            quote.InsertedNodes.Add(expr);

            return null;
        }

        /// <summary>
        /// Writes the given code to the output as if it were a C# value.
        /// </summary>
        public static object operator >(Quote quote, string code) => quote > code.Syntax<ExpressionSyntax>();

        /// <summary>
        /// Returns the given expression as if it were a C# value.
        /// </summary>
        public static object operator <(Quote quote, ExpressionSyntax expr)
        {
            quote.InsertedNodes.Add(expr);

            return null;
        }

        /// <summary>
        /// Writes the given code to the output as if it were a C# value.
        /// </summary>
        public static object operator <(Quote quote, string code) => quote > code.Syntax<ExpressionSyntax>();
        #endregion

        #region Mixin
        /// <summary>
        /// Inserts the given <see cref="string"/> in the syntax tree, as if it
        /// were an actual statement.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void Mixin(string code, Quote quote = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            Meta.EnsureCompiling();

            quote.Add(SyntaxFactory.ParseStatement(code).WithSemicolon());
        }

        /// <summary>
        /// Inserts the given <see cref="string"/> in the syntax tree, as if it
        /// were an actual expression.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static T Mixin<T>(string code, Quote quote = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            Meta.EnsureCompiling();

            quote.Add(SyntaxFactory.ParseExpression(code));

            return default(T);
        }

        /// <summary>
        /// Inserts the given <paramref name="node"/> in the syntax tree, as if it
        /// were an actual statement.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void Mixin(StatementSyntax node, Quote quote = null)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            Meta.EnsureCompiling();

            quote.Add(node);
        }

        /// <summary>
        /// Inserts the given <paramref name="node"/> in the syntax tree, as if it
        /// were an actual expression.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static T Mixin<T>(ExpressionSyntax node, Quote quote = null)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            Meta.EnsureCompiling();

            quote.Add(node);

            return default(T);
        }
        #endregion
    }

    /// <summary>
    /// Represents a quote, used by macros, used by macros
    /// to insert code dynamically in a method body.
    /// </summary>
    public sealed class Quote<T> : Quote
    {
        internal Quote(Quote quote, int nth) : base(quote, nth)
        {
        }

        /// <summary>
        /// Returns a new identical <see cref="Quote{T}"/>, with an additional
        /// inserted node.
        /// </summary>
        internal new Quote<T> Push(SyntaxNode node)
        {
            InsertedNodes.Add(node);

            return this;
        }

        /// <summary>
        /// Implicitely converts an expression of type T into a quote.
        /// </summary>
        public static implicit operator Quote<T>(T value)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Implicitely converts an expression of type T into a quote.
        /// </summary>
        public static implicit operator T(Quote<T> quote) => default(T);

        /// <inheritdoc cref="Quote.operator +(Quote,StatementSyntax)" />
        public static Quote<T> operator +(Quote<T> quote, StatementSyntax stmt) => quote.Push(stmt);

        /// <inheritdoc cref="Quote.operator +(Quote,ExpressionSyntax)" />
        public static Quote<T> operator +(Quote<T> quote, ExpressionSyntax expr) => quote.Push(expr);

        /// <inheritdoc cref="Quote.operator +(Quote,string)" />
        public static Quote<T> operator +(Quote<T> quote, string code) => quote + code.Syntax<ExpressionSyntax>();
    }
}
