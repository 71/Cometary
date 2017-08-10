using System;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    ///   Represents an <see cref="Exception"/> wrapping a <see cref="Microsoft.CodeAnalysis.Diagnostic"/>.
    /// </summary>
    public class DiagnosticException : Exception
    {
        /// <summary>
        ///   <see cref="DiagnosticDescriptor"/> that describes the diagnostic generated
        ///   when throwing a <see cref="DiagnosticException"/> using a <see cref="string"/> and a <see cref="Location"/>.
        /// </summary>
        public readonly DiagnosticDescriptor DefaultDescriptor = new DiagnosticDescriptor(
            nameof(DiagnosticException) + "Error", "Exception encountered", "{0}", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Gets the diagnostic that led to this exception.
        /// </summary>
        public Diagnostic Diagnostic { get; }
        
        /// <summary>
        ///   Creates a new <see cref="DiagnosticException"/>, given the diagnostic
        ///   that led to the error.
        /// </summary>
        public DiagnosticException(Diagnostic diagnostic) : base(diagnostic.GetMessage())
        {
            Diagnostic = diagnostic;
        }

        /// <summary>
        ///   Creates a new <see cref="DiagnosticException"/>, given a message
        ///   and the location in which the error happened.
        /// </summary>
        public DiagnosticException(string message, Location location) : this(message, null, location)
        {
        }

        /// <summary>
        ///   Creates a new <see cref="DiagnosticException"/>, given a message, the inner
        ///   exception that led to the error, and the logical location of the error.
        /// </summary>
        public DiagnosticException(string message, Exception innerException, Location location) : base(message, innerException)
        {
            Diagnostic = Diagnostic.Create(DefaultDescriptor, location ?? Location.None, message.Filter());
        }

        /// <inheritdoc />
        public override string ToString() => $"[{Diagnostic.Location}] {Message}";
    }
}
