using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Extensions
{
    partial class SyntaxExtensions
    {
        /// <summary>
        /// Gets the <see cref="IOperation"/> associated with the given <see cref="SyntaxNode"/>.
        /// </summary>
        public static IOperation Operation(this SyntaxNode node) => node.SyntaxTree.Model()?.GetOperation(node);

        /// <summary>
        /// Gets the <see cref="IOperation"/> associated with the given <see cref="SyntaxNode"/>.
        /// </summary>
        public static TOperation Operation<TOperation>(this SyntaxNode node) where TOperation : class, IOperation => node.Operation() as TOperation;


        /// <summary>
        /// Gets the <see cref="ISymbol"/> associated with the given declaration <see cref="SyntaxNode"/>.
        /// </summary>
        public static ISymbol Symbol(this SyntaxNode declaration) => declaration.SyntaxTree.Model()?.GetDeclaredSymbol(declaration);

        /// <summary>
        /// Gets the <see cref="ISymbol"/> associated with the given declaration <see cref="SyntaxNode"/>.
        /// </summary>
        public static TSymbol Symbol<TSymbol>(this SyntaxNode declaration) where TSymbol : class, ISymbol => declaration.Symbol() as TSymbol;

        /// <summary>
        /// Gets the <see cref="ITypeSymbol"/> associated with the given <see cref="TypeDeclarationSyntax"/>.
        /// </summary>
        public static ITypeSymbol Symbol(this TypeDeclarationSyntax type) => type.Symbol<ITypeSymbol>();

        /// <summary>
        /// Gets the <see cref="IMethodSymbol"/> associated with the given <see cref="MethodDeclarationSyntax"/>.
        /// </summary>
        public static IMethodSymbol Symbol(this MethodDeclarationSyntax method)
            => method.DeclaringType()
                     .Symbol()?
                     .GetMembers(method.Identifier.Text)
                     .OfType<IMethodSymbol>()
                     .FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="IFieldSymbol"/> associated with the given <see cref="FieldDeclarationSyntax"/>.
        /// </summary>
        public static IFieldSymbol Symbol(this FieldDeclarationSyntax field)
            => field.DeclaringType()
                    .Symbol()?
                    .GetMembers(field.Identifier().Text)
                    .OfType<IFieldSymbol>()
                    .FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="IPropertySymbol"/> associated with the given <see cref="PropertyDeclarationSyntax"/>.
        /// </summary>
        public static IPropertySymbol Symbol(this PropertyDeclarationSyntax property)
            => property.DeclaringType()
                       .Symbol()?
                       .GetMembers(property.Identifier.Text)
                       .OfType<IPropertySymbol>()
                       .FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="IParameterSymbol"/> associated with the given <see cref="ParameterSyntax"/>.
        /// </summary>
        public static IParameterSymbol Symbol(this ParameterSyntax parameter)
        {
            MethodDeclarationSyntax method = parameter.DeclaringMethod();
            int index = method.ParameterList.Parameters.IndexOf(parameter);

            return method.Symbol().Parameters[index];
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> associated with the given <see cref="IMethodSymbol"/>.
        /// </summary>
        public static MethodInfo Info(this IMethodSymbol method)
        {
            // Holy shit, to support local methods, we have to construct a map of
            // all generic parameters. Fuck.
            TypeInfo declaringType = method.ContainingType.Info();
            var parameters = method.Parameters;

            IEnumerable<MethodInfo> possibleMethods;

            if (method.ContainingSymbol is IMethodSymbol declaringMethod)
            {
                string prefix = $"<{declaringMethod.Name}>";
                possibleMethods = declaringType.DeclaredMethods.Where(x => x.Name.StartsWith(prefix) && x.Name.Contains(method.MetadataName));
            }
            else
            {
                possibleMethods = declaringType.GetDeclaredMethods(method.MetadataName);
            }

            MethodInfo result = possibleMethods.First(IsTargetMethod);

            if (result.IsGenericMethod)
                result = result.MakeGenericMethod(ConstructGenericMethodArguments());

            return result;

            // Finds a matching method.
            bool IsTargetMethod(MethodInfo info)
            {
                if (info.IsStatic != method.IsStatic)
                    return false;

                ParameterInfo[] pis = info.GetParameters();

                if (pis.Length != parameters.Length)
                    return false;

                for (int i = 0; i < pis.Length; i++)
                {
                    if (!pis[i].ParameterType.IsGenericParameter && pis[i].ParameterType.Name != parameters[i].Type.Name)
                        return false;
                }

                return true;
            }

            Type[] ConstructGenericMethodArguments()
            {
                Type[] typeArgs = result.GetGenericArguments();

                for (int i = 0; i < typeArgs.Length; i++)
                {
                    Type typeArg = typeArgs[i];

                    if (!typeArg.IsGenericParameter)
                        continue;

                    typeArgs[i] = FindMatchingType(typeArg, method) ?? typeArg.GetTypeInfo().BaseType ?? typeof(object);
                }

                return typeArgs;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Type FindMatchingType(Type genericType, ISymbol ctx)
        {
            if (!genericType.IsGenericParameter)
                return genericType;

            string name = genericType.Name;

            for(;;)
            {
                ImmutableArray<ITypeParameterSymbol> typeParameters;
                ImmutableArray<ITypeSymbol> typeArguments;

                if (ctx is IMethodSymbol method)
                {
                    typeParameters = method.TypeParameters;
                    typeArguments  = method.TypeArguments;
                }
                else if (ctx is INamedTypeSymbol type)
                {
                    typeParameters = type.TypeParameters;
                    typeArguments  = type.TypeArguments;
                }

                for (int i = 0; i < typeParameters.Length; i++)
                {
                    ITypeParameterSymbol typeParameter = typeParameters[i];

                    if (typeParameter.Name == name)
                        return typeArguments[i].Info()?.AsType();
                }

                ctx = ctx.ContainingSymbol;

                if (ctx == null)
                    return null;
            }
        }
    }
}
