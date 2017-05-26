using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Rest
{
    using Attributes;
    using Extensions;

    /// <summary>
    /// Defines a method that makes an HTTP call.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpMethodAttribute : Attribute, IMethodVisitor
    {
        /// <summary>
        /// Gets the HTTP method used to access the resource.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Gets the path (absolute or relative) of the resource to access.
        /// </summary>
        public string Path { get; }

        /// <inheritdoc cref="HttpMethodAttribute" />
        public HttpMethodAttribute(string method, string path)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Path = path;
        }

        /// <summary>
        /// Gets the <see cref="HttpClient"/> syntax for the given <paramref name="node"/>.
        /// </summary>
        protected virtual ExpressionSyntax GetHttpClientSyntax(MethodDeclarationSyntax node)
        {
            return $"new globals::{typeof(HttpClient).FullName}()".Syntax<ExpressionSyntax>();
        }

        /// <summary>
        /// Gets the <see cref="HttpRequestMessage"/> syntax for the given <paramref name="node"/>.
        /// </summary>
        protected virtual ExpressionSyntax GetHttpRequestMessageSyntax(MethodDeclarationSyntax node)
        {
            return ($"new globals::{typeof(HttpRequestMessage).FullName}("
                  + $"new globals::{typeof(HttpMethod)}(\"{Method}\"),"
                  + $"\"{Path}\")").Syntax<ExpressionSyntax>();
        }

        /// <summary>
        /// Gets the different arguments passed to <see cref="HttpClient.SendAsync(HttpRequestMessage)"/>.
        /// </summary>
        protected virtual IEnumerable<ArgumentSyntax> GetArgumentsSyntax(MethodDeclarationSyntax node)
        {
            yield return F.Argument(GetHttpRequestMessageSyntax(node));

            foreach (ParameterSyntax parameter in node.ParameterList.Parameters)
            {
                if (parameter.Symbol().Type.Name == typeof(CancellationToken).FullName)
                {
                    yield return F.Argument(F.IdentifierName(parameter.Identifier));
                    yield break;
                }
            }
        }

        /// <summary>
        /// Visits the given method.
        /// </summary>
        public virtual MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            if (!node.IsExtern())
                throw new ProcessingException(node, "The given method must be extern.");

            return node.NotExtern(
                F.AwaitExpression(
                    F.InvocationExpression(
                        GetHttpClientSyntax(node),
                        F.ArgumentList(
                            F.SeparatedList(GetArgumentsSyntax(node))
                        )
                    )
                )
            );
        }
    }
}
