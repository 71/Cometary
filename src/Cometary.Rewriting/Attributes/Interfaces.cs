using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Attributes
{
    #region Member visitors
    /// <summary>
    ///   Base interface for all visitors.
    /// </summary>
    public interface ICometaryVisitor
    {
    }

    /// <summary>
    ///   Defines an attribute that visits methods.
    /// </summary>
    public interface IMethodVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="method"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the method will be removed.
        /// </para>
        /// </summary>
        MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits fields.
    /// </summary>
    public interface IFieldVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="field"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the field will be removed.
        /// </para>
        /// </summary>
        FieldDeclarationSyntax Visit(FieldInfo field, FieldDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits propertys.
    /// </summary>
    public interface IPropertyVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="property"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the property will be removed.
        /// </para>
        /// </summary>
        PropertyDeclarationSyntax Visit(PropertyInfo property, PropertyDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits events.
    /// </summary>
    public interface IEventVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="event"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the event will be removed.
        /// </para>
        /// </summary>
        EventDeclarationSyntax Visit(EventInfo @event, EventDeclarationSyntax node);
    }
    #endregion

    #region Type-derived visitors
    /// <summary>
    ///   Defines an attribute that visits classes.
    /// </summary>
    public interface IClassVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="type"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the type will be removed.
        /// </para>
        /// </summary>
        ClassDeclarationSyntax Visit(TypeInfo type, ClassDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits structs.
    /// </summary>
    public interface IStructVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="struct"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the struct will be removed.
        /// </para>
        /// </summary>
        StructDeclarationSyntax Visit(TypeInfo @struct, StructDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits delegates.
    /// </summary>
    public interface IDelegateVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="delegate"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the delegate will be removed.
        /// </para>
        /// </summary>
        DelegateDeclarationSyntax Visit(TypeInfo @delegate, DelegateDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits enums.
    /// </summary>
    public interface IEnumVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="enum"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the enum will be removed.
        /// </para>
        /// </summary>
        EnumDeclarationSyntax Visit(TypeInfo @enum, EnumDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits interfaces.
    /// </summary>
    public interface IInterfaceVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="interface"/>, by modifying
        ///   its declaration syntax.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the interface will be removed.
        /// </para>
        /// </summary>
        InterfaceDeclarationSyntax Visit(TypeInfo @interface, InterfaceDeclarationSyntax node);
    }
    #endregion

    #region Misc visitors
    /// <summary>
    ///   Defines an attribute that visits parameters.
    /// </summary>
    public interface IParameterVisitor : ICometaryVisitor
    {
        /// <summary>
        ///   Transforms the given <paramref name="parameter"/>, by modifying
        ///   its declaration syntax.
        /// </summary>
        MethodDeclarationSyntax Visit(ParameterInfo parameter, ParameterSyntax syntax, MethodDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits generic parameters.
    /// </summary>
    public interface IGenericParameterVisitor : ICometaryVisitor
    {
        /// <summary>
        ///   Transforms the given <paramref name="parameter"/>, by modifying
        ///   its declaration syntax.
        /// </summary>
        GenericNameSyntax Visit(ParameterInfo parameter, GenericNameSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits return values.
    /// </summary>
    public interface IReturnValueVisitor : ICometaryVisitor
    {
        /// <summary>
        ///   Transforms the given return value, by modifying
        ///   its declaration syntax.
        /// </summary>
        MethodDeclarationSyntax Visit(TypeInfo parameterType, MethodDeclarationSyntax node);
    }

    /// <summary>
    ///   Defines an attribute that visits assemblies.
    /// </summary>
    public interface IAssemblyVisitor : ICometaryVisitor
    {
        /// <summary>
        ///   Transforms the given <paramref name="assembly"/>, by modifying
        ///   its declaration syntax.
        /// </summary>
        CSharpCompilation Visit(Assembly assembly, CSharpCompilation node);
    }
    #endregion
}
