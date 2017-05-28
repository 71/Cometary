using System;

namespace Cometary.Rest
{
    
    /// <summary>
    /// Defines a method that makes a GET request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : HttpMethodAttribute
    {
        /// <inheritdoc />
        public GetAttribute(string path) : base("GET", path)
        {
        }
    }
	
    /// <summary>
    /// Defines a method that makes a POST request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : HttpMethodAttribute
    {
        /// <inheritdoc />
        public PostAttribute(string path) : base("POST", path)
        {
        }
    }
	
    /// <summary>
    /// Defines a method that makes a PUT request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : HttpMethodAttribute
    {
        /// <inheritdoc />
        public PutAttribute(string path) : base("PUT", path)
        {
        }
    }
	
    /// <summary>
    /// Defines a method that makes a OPTIONS request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OptionsAttribute : HttpMethodAttribute
    {
        /// <inheritdoc />
        public OptionsAttribute(string path) : base("OPTIONS", path)
        {
        }
    }
	
}
