using Microsoft.CodeAnalysis;

namespace Cometary
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
        ///   its symbol.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the method will be removed.
        /// </para>
        /// </summary>
        IMethodSymbol Visit(IMethodSymbol method);
    }

    /// <summary>
    ///   Defines an attribute that visits fields.
    /// </summary>
    public interface IFieldVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="field"/>, by modifying
        ///   its symbol.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the field will be removed.
        /// </para>
        /// </summary>
        IFieldSymbol Visit(IFieldSymbol field);
    }

    /// <summary>
    ///   Defines an attribute that visits propertys.
    /// </summary>
    public interface IPropertyVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="property"/>, by modifying
        ///   its symbol.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the property will be removed.
        /// </para>
        /// </summary>
        IPropertySymbol Visit(IPropertySymbol property);
    }

    /// <summary>
    ///   Defines an attribute that visits events.
    /// </summary>
    public interface IEventVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="event"/>, by modifying
        ///   its symbol.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the event will be removed.
        /// </para>
        /// </summary>
        IEventSymbol Visit(IEventSymbol @event);
    }
    #endregion

    #region Type-derived visitors
    /// <summary>
    ///   Defines an attribute that visits classes.
    /// </summary>
    public interface ITypeVisitor : ICometaryVisitor
    {
        /// <summary>
        /// <para>
        ///   Transforms the given <paramref name="type"/>, by modifying
        ///   its symbol.
        /// </para>
        /// <para>
        ///   If <see langword="null"/> is returned, the type will be removed.
        /// </para>
        /// </summary>
        INamedTypeSymbol Visit(INamedTypeSymbol type);
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
        ///   its symbol.
        /// </summary>
        IParameterSymbol Visit(IParameterSymbol parameter);
    }

    /// <summary>
    ///   Defines an attribute that visits generic parameters.
    /// </summary>
    public interface ITypeParameterVisitor : ICometaryVisitor
    {
        /// <summary>
        ///   Transforms the given <paramref name="parameter"/>, by modifying
        ///   its symbol.
        /// </summary>
        ITypeParameterSymbol Visit(ITypeParameterSymbol parameter);
    }

    /// <summary>
    ///   Defines an attribute that visits return values.
    /// </summary>
    public interface IReturnValueVisitor : ICometaryVisitor
    {
        /// <summary>
        ///   Transforms the given return value, by modifying
        ///   its symbol.
        /// </summary>
        IMethodSymbol Visit(ITypeSymbol parameterType, IMethodSymbol node);
    }
    #endregion
}
