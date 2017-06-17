using System;
using System.ComponentModel;

namespace Cometary.Contracts
{
    /// <summary>
    /// Represents an <see cref="Exception"/> thrown by <see cref="Requires"/>,
    /// that contains additional informations about the error,
    /// including its <see cref="string"/> representation.
    /// </summary>
    public sealed class AssertionException : Exception
    {
        /// <summary>
        /// Gets the file in which the assertion failed.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets the line at which the assertion failed.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column at which the assertion failed.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the <see cref="string"/> representation of the
        /// expression that led to the failed assertion.
        /// </summary>
        public string Expression { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public AssertionException(string msg, string expr, string file, int line, int col) : base(msg)
        {
            Expression = expr;
            File = file;

            Line = line;
            Column = col;
        }

        /// <inheritdoc />
        public override string ToString() => Message.Replace("expression", $"\"{Expression}\"");
    }
}
