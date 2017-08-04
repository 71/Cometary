using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   <see cref="CSharpSyntaxRewriter"/> that invokes a callback
    ///   when it visits a <see cref="SyntaxNode"/>.
    /// </summary>
    internal sealed class SyntaxTreeRewriter : CSharpSyntaxRewriter
    {
        private readonly Func<SyntaxNode, SyntaxNode> callback;

        internal SyntaxTreeRewriter(Func<SyntaxNode, SyntaxNode> callback)
        {
            this.callback = callback;
        }

        /// <summary>
        ///   Invokes the previously given <see cref="callback"/>, and returns its result.
        /// </summary>
        public override SyntaxNode Visit(SyntaxNode node)
        {
            return base.Visit(callback(node));
        }
    }
}
