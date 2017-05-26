using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Cometary
{
    /// <summary>
    /// Represents an <see cref="Exception"/> thrown when processing
    /// a syntax node.
    /// </summary>
    public class ProcessingException : Exception
    {
        /// <summary>
        /// Gets the node that caused the issue.
        /// </summary>
        public SyntaxNode Node { get; }

        /// <summary>
        /// Gets the text span that caused the issue.
        /// </summary>
        public TextSpan Span { get; }

        /// <inheritdoc />
        public ProcessingException(SyntaxNode node, string message) : base(message)
        {
            Node = node;
            Span = node.FullSpan;
        }

        /// <inheritdoc />
        public ProcessingException(SyntaxNode node, string message, Exception innerException) : base(message, innerException)
        {
            Node = node;
            Span = node.FullSpan;
        }

        /// <inheritdoc />
        public ProcessingException(TextSpan span, string message) : base(message)
        {
            Span = span;
        }

        /// <inheritdoc />
        public ProcessingException(TextSpan span, string message, Exception innerException) : base(message, innerException)
        {
            Span = span;
        }

        /// <inheritdoc />
        public override string ToString() => Span.IsEmpty ? Message : $"{Span} {Message}";
    }
}
