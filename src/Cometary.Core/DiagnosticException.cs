using System;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    public class DiagnosticException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly DiagnosticDescriptor DefaultDescriptor = new DiagnosticDescriptor(
            Common.DiagnosticsPrefix + "EXC001", "Exception encountered", "{0}", "Editing", DiagnosticSeverity.Error, true);

        /// <summary>
        /// 
        /// </summary>
        public Diagnostic Diagnostic { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public DiagnosticException(Diagnostic diagnostic) : base(diagnostic.GetMessage())
        {
            Diagnostic = diagnostic;
        }

        /// <summary>
        /// 
        /// </summary>
        public DiagnosticException(string message, Location location) : this(message, null, location)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DiagnosticException(string message, Exception innerException, Location location) : base(message, innerException)
        {
            Diagnostic = Diagnostic.Create(DefaultDescriptor, location ?? Location.None, message);
        }
    }
}
