using System;

namespace Cometary.Tests
{
    /// <summary>
    ///   
    /// </summary>
    public sealed class AssertionException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 
        /// </summary>
        public string File { get; }

        /// <summary>
        /// 
        /// </summary>
        public AssertionException(string message, string expression, string file, int position, int width) : base(message)
        {
            Expression = expression;
            File = file;
            Position = position;
            Width = width;
        }
    }
}
