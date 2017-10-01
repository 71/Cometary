using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Cometary
{
    /// <summary>
    ///   Represents a message logged whilst processing an assembly.
    /// </summary>
    public sealed class ProcessingMessage
    {
        /// <summary>
        ///   Gets the message as a <see cref="string"/>.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///   Gets the <see cref="string"/> representation of the sender of the message.
        /// </summary>
        public string Sender { get; }

        /// <summary>
        ///   Gets the <see cref="SyntaxNode"/> associated with this message.
        /// </summary>
        public SyntaxNode Node { get; }

        /// <summary>
        ///   Gets the <see cref="TextSpan"/> of the <see cref="SyntaxNode"/>
        ///   associated with this message, if any.
        /// </summary>
        public TextSpan Span { get; }

        /// <summary>
        ///   Creates a new <see cref="ProcessingMessage"/>, with an optional
        ///   related <see cref="SyntaxNode"/>.
        /// </summary>
        public ProcessingMessage(string message, SyntaxNode node = null, string sender = null)
        {
            Message = message;
            Node = node;
            Sender = sender;

            if (node != null)
                Span = node.Span;
        }

        /// <summary>
        ///   Creates a new <see cref="ProcessingMessage"/>, with an optional
        ///   related <see cref="SyntaxNode"/>.
        /// </summary>
        public ProcessingMessage(string message, TextSpan span, string sender = null)
        {
            Message = message;
            Sender = sender;
            Span = span;
        }

        /// <inheritdoc />
        public override string ToString() => Span.IsEmpty ? Message : $"{Span} {Message}";
    }
}
