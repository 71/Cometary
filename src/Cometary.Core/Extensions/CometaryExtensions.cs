using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Provides utilities for symbols.
    /// </summary>
    public static partial class CometaryExtensions
    {
        /// <summary>
        ///   Returns a new <see cref="Location"/> whose <see cref="Location.SourceTree"/>
        ///   and <see cref="Location.SourceSpan"/> properties match the <see cref="SyntaxReference.SyntaxTree"/>
        ///   and <see cref="SyntaxReference.Span"/> properties.
        /// </summary>
        public static Location ToLocation(this SyntaxReference syntaxReference)
        {
            return Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);
        }

        #region GetCorresponding*
        /// <summary>
        ///   Gets the <see cref="BindingFlags"/> corresponding to the
        ///   <see cref="Accessibility"/> of the given <paramref name="symbol"/>.
        /// </summary>
        public static BindingFlags GetCorrespondingBindingFlags(this ISymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            BindingFlags result = 0;

            if (symbol.DeclaredAccessibility == Accessibility.Public)
                result |= BindingFlags.Public;
            else
                result |= BindingFlags.NonPublic;

            if (symbol.IsStatic)
                result |= BindingFlags.Static;
            else
                result |= BindingFlags.Instance;

            return result;
        }

        /// <summary>
        ///   Gets the <see cref="Type"/> corresponding to the given
        ///   <paramref name="symbol"/>.
        /// </summary>
        public static Type GetCorrespondingType(this ITypeSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            switch (symbol.TypeKind)
            {
                case TypeKind.Array:
                    return ((IArrayTypeSymbol)symbol).ElementType.GetCorrespondingType().MakeArrayType();
                case TypeKind.Enum:
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Interface:
                case TypeKind.Struct:
                    break;
                case TypeKind.Pointer:
                    return ((IPointerTypeSymbol)symbol).PointedAtType.GetCorrespondingType().MakePointerType();
                case TypeKind.TypeParameter:
                    ITypeParameterSymbol typeParameter = (ITypeParameterSymbol)symbol;
                    return typeParameter.DeclaringType.GetCorrespondingType().GenericTypeArguments[typeParameter.Ordinal];
                default:
                    throw new ArgumentOutOfRangeException();
            }

            INamedTypeSymbol named = (INamedTypeSymbol)symbol;
            Type type = named.ContainingNamespace.IsGlobalNamespace
                ? Type.GetType($"{named.MetadataName}, {named.ContainingAssembly.MetadataName}")
                : Type.GetType($"{named.ContainingNamespace}.{named.MetadataName}, {named.ContainingAssembly.MetadataName}");

            if (type == null)
                return null;

            int typeArgsLength = type.GenericTypeArguments.Length;

            if (typeArgsLength == 0)
                return type;

            Type[] typeArgs = new Type[typeArgsLength];

            for (int i = 0; i < typeArgsLength; i++)
            {
                typeArgs[i] = named.TypeArguments[i].GetCorrespondingType();
            }

            return type.MakeGenericType(typeArgs);
        }

        /// <summary>
        ///   Gets the <see cref="MethodBase"/> corresponding to the given
        ///   <paramref name="symbol"/>.
        /// </summary>
        public static MethodBase GetCorrespondingMethod(this IMethodSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            Type declaringType = symbol.ContainingType.GetCorrespondingType();

            if (declaringType == null)
                return null;

            BindingFlags flags = symbol.GetCorrespondingBindingFlags();

            IEnumerable<MethodBase> possibleMethods;

            switch (symbol.MethodKind)
            {
                case MethodKind.Constructor:
                case MethodKind.StaticConstructor:
                    possibleMethods = declaringType.GetConstructors(flags);
                    break;
                default:
                    possibleMethods = declaringType.GetMethods(flags);
                    break;
            }

            ImmutableArray<IParameterSymbol> symbolParameters = symbol.Parameters;

            foreach (MethodBase method in possibleMethods)
            {
                if (method.Name != symbol.Name)
                    continue;

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length != symbolParameters.Length)
                    continue;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != symbolParameters[i].Type.GetCorrespondingType())
                        goto Continue;
                }

                return method;

                Continue:;
            }

            return null;
        }

        /// <summary>
        ///   Gets the <see cref="FieldInfo"/> corresponding to the given
        ///   <paramref name="symbol"/>.
        /// </summary>
        public static FieldInfo GetCorrespondingField(this IFieldSymbol symbol)
        {
            return symbol.ContainingType.GetCorrespondingType().GetField(symbol.Name, symbol.GetCorrespondingBindingFlags());
        }

        /// <summary>
        ///   Gets the <see cref="PropertyInfo"/> corresponding to the given
        ///   <paramref name="symbol"/>.
        /// </summary>
        public static PropertyInfo GetCorrespondingProperty(this IPropertySymbol symbol)
        {
            return symbol.ContainingType.GetCorrespondingType().GetProperty(symbol.Name, symbol.GetCorrespondingBindingFlags());
        }

        /// <summary>
        ///   Gets the <see cref="EventInfo"/> corresponding to the given
        ///   <paramref name="symbol"/>.
        /// </summary>
        public static EventInfo GetCorrespondingEvent(this IEventSymbol symbol)
        {
            return symbol.ContainingType.GetCorrespondingType().GetEvent(symbol.Name, symbol.GetCorrespondingBindingFlags());
        }
        #endregion

        #region Attributes
        /// <summary>
        ///   Returns the value of the specified <see cref="TypedConstant"/>,
        ///   even if its kind is <see cref="TypedConstantKind.Array"/>.
        /// </summary>
        public static object GetValue(this TypedConstant typedConstant)
        {
            switch (typedConstant.Kind)
            {
                case TypedConstantKind.Type:
                    return ((ITypeSymbol)typedConstant.Value).GetCorrespondingType();
                case TypedConstantKind.Primitive:
                case TypedConstantKind.Enum:
                    return typedConstant.Value;
                case TypedConstantKind.Array:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            ImmutableArray<TypedConstant> elements = typedConstant.Values;
            Array array = Array.CreateInstance(typedConstant.Type.GetCorrespondingType().GetElementType(), elements.Length);

            for (int i = 0; i < elements.Length; i++)
            {
                array.SetValue(elements[i].GetValue(), i);
            }

            return array;
        }

        /// <summary>
        ///   Returns a constructed attribute of type <typeparamref name="T"/>.
        /// </summary>
        public static T Construct<T>(this AttributeData attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            // Make args array
            ImmutableArray<TypedConstant> constantArgs = attribute.ConstructorArguments;
            object[] args = new object[constantArgs.Length];

            for (int i = 0; i < constantArgs.Length; i++)
            {
                args[i] = constantArgs[i].GetValue();
            }

            // Find ctor, and invoke it
            ConstructorInfo constructor = attribute.AttributeConstructor.GetCorrespondingMethod() as ConstructorInfo;

            if (constructor == null)
                throw new DiagnosticException($"Cannot find a constructor matching {attribute.AttributeConstructor}.", attribute.ApplicationSyntaxReference.ToLocation());

            T result = (T)constructor.Invoke(args);

            // Set named args
            var namedArgs = attribute.NamedArguments;

            if (namedArgs.Length == 0)
                return result;

            Type attrType = constructor.DeclaringType;

            for (int i = 0; i < namedArgs.Length; i++)
            {
                var namedArg = namedArgs[i];
                MemberInfo correspondingMember = attrType.GetMember(namedArg.Key, BindingFlags.Instance | BindingFlags.Public)[0];

                switch (correspondingMember)
                {
                    case PropertyInfo prop:
                        prop.SetValue(result, namedArg.Value.GetValue());
                        break;
                    case FieldInfo field:
                        field.SetValue(result, namedArg.Value.GetValue());
                        break;
                }
            }

            // Return fully constructed attr
            return result;
        }

        /// <summary>
        ///   Returns all attributes implementing the <typeparamref name="T"/> interface.
        /// </summary>
        public static IEnumerable<T> FindAttributesOfInterface<T>(this ImmutableArray<AttributeData> attributes)
        {
            Type interfType = typeof(T);

            if (!interfType.GetTypeInfo().IsInterface)
                throw new ArgumentException("Expected an interface.");

            string metadataName = interfType.IsConstructedGenericType
                ? interfType.GetGenericTypeDefinition().FullName
                : interfType.FullName;

            for (int i = 0; i < attributes.Length; i++)
            {
                AttributeData data = attributes[i];
                ImmutableArray<INamedTypeSymbol> interfaces = data.AttributeClass.AllInterfaces;

                for (int o = 0; o < interfaces.Length; o++)
                {
                    INamedTypeSymbol interf = interfaces[o];

                    if (interf.MetadataName != metadataName)
                        continue;

                    // We got this far, so we have a valid attribute; construct it.
                    yield return data.Construct<T>();
                    break;
                }
            }
        }

        /// <summary>
        ///   Returns all attributes of type <typeparamref name="T"/>.
        /// </summary>
        public static IEnumerable<T> FindAttributesOfType<T>(this ImmutableArray<AttributeData> attributes, bool allowInherited = true)
        {
            Type type = typeof(T);

            if (type.GetTypeInfo().IsInterface)
                throw new ArgumentException("Expected a type.");

            string metadataName = type.IsConstructedGenericType
                ? type.GetGenericTypeDefinition().Name
                : type.Name;

            for (int i = 0; i < attributes.Length; i++)
            {
                AttributeData data = attributes[i];
                ITypeSymbol typeSymbol = data.AttributeClass;

                if (!allowInherited && typeSymbol.MetadataName == metadataName)
                {
                    yield return data.Construct<T>();
                }
                else if (allowInherited)
                {
                    while (typeSymbol != null)
                    {
                        if (typeSymbol.MetadataName == metadataName)
                        {
                            yield return data.Construct<T>();
                            break;
                        }

                        typeSymbol = typeSymbol.BaseType;
                    }
                }
            }
        }

        /// <summary>
        ///   Returns all attributes assignable to <typeparamref name="T"/>.
        /// </summary>
        public static IEnumerable<T> FindAttributes<T>(this ImmutableArray<AttributeData> attributes)
        {
            return typeof(T).GetTypeInfo().IsInterface
                ? attributes.FindAttributesOfInterface<T>()
                : attributes.FindAttributesOfType<T>();
        }
        #endregion
    }
}
