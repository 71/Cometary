using System;
using System.Collections.Generic;
using System.Reflection;
using Cometary.Attributes;
using Cometary.Helpers;
using Cometary.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;
using System.IO;

namespace Cometary.Visiting
{
    /// <summary>
    /// Defines a class that visits an assembly's members.
    /// </summary>
    public abstract class AssemblyVisitor
        : ITypeVisitor, IDelegateVisitor, IEnumVisitor, IInterfaceVisitor,
          IMethodVisitor, IFieldVisitor, IPropertyVisitor,
          IAssemblyVisitor, IParameterVisitor
    {
        /// <summary>
        /// Defines the current state of the compilation.
        /// </summary>
        public enum CompilationState
        {
            /// <summary>
            /// The assembly has been compiled, but not yet fully visited.
            /// </summary>
            Loaded,

            /// <summary>
            /// The assembly has been compiled and emitted.
            /// </summary>
            Visited
        }

        /// <summary>
        /// Gets the priority at which the assembly visitor will be invoked.
        /// <para>
        /// A higher priority means a faster invocation.
        /// </para>
        /// </summary>
        public abstract float Priority { get; }

        /// <summary>
        /// Gets the assembly in that is being processed.
        /// </summary>
        public Assembly Assembly { get; internal set; }

        /// <summary>
        /// Gets the compilation corresponding to the assembly being processed.
        /// </summary>
        public CSharpCompilation Compilation { get; internal set; }

        /// <inheritdoc />
        public virtual MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual TypeDeclarationSyntax Visit(TypeInfo type, TypeDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual PropertyDeclarationSyntax Visit(PropertyInfo property, PropertyDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual FieldDeclarationSyntax Visit(FieldInfo field, FieldDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual DelegateDeclarationSyntax Visit(TypeInfo type, DelegateDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual EventDeclarationSyntax Visit(EventInfo @event, EventDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual ParameterSyntax Visit(ParameterInfo parameter, ParameterSyntax node) => node;

        /// <inheritdoc />
        public virtual EnumDeclarationSyntax Visit(TypeInfo @enum, EnumDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual InterfaceDeclarationSyntax Visit(TypeInfo @interface, InterfaceDeclarationSyntax node) => node;

        /// <summary>
        /// Transforms the specified compilation.
        /// <para>
        /// This method is invoked after the original <see cref="SyntaxTree"/> transformation.
        /// </para>
        /// </summary>
        public virtual CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation) => compilation;

        /// <summary>
        /// Transforms the given streams.
        /// </summary>
        /// <param name="assemblyStream">The <see cref="Stream"/> containing the emitted assembly.</param>
        /// <param name="symbolsStream">The <see cref="Stream"/> containing the symbols of the emitted assembly.</param>
        /// <param name="state">The state of the compilation.</param>
        public virtual void Visit(MemoryStream assemblyStream, MemoryStream symbolsStream, CompilationState state) { }

        /// <summary>
        /// Transforms the specified syntax tree.
        /// </summary>
        internal static void Visit(SyntaxTree syntaxTree, IList<AssemblyVisitor> visitors)
        {
            SyntaxNode root = syntaxTree.GetRoot();

            if (!(root is CompilationUnitSyntax unit))
                return;

            TMemberSyntax DoTopLevel<TMemberInfo, TMemberSyntax>(TMemberInfo info, TMemberSyntax member, Func<AssemblyVisitor, TMemberInfo, TMemberSyntax, TMemberSyntax> method) where TMemberSyntax : MemberDeclarationSyntax
            {
                // Visit a top-level declaration (not in a type)

                TMemberSyntax newMember = member;

                foreach (AssemblyVisitor visitor in visitors)
                {
                    newMember = method(visitor, info, newMember);

                    if (newMember == null)
                        return null;
                }

                return newMember;
            }

            TypeDeclarationSyntax DoType(TypeInfo typeInfo, TypeDeclarationSyntax oldType)
            {
                // Visit every declaration inside a type
                TypeDeclarationSyntax type = oldType;
                int index = 0;

                foreach (AssemblyVisitor visitor in visitors)
                {
                    type = visitor.Visit(typeInfo, type);

                    if (type == null)
                        return null;
                }

                void DoMember<TInfo, TSyntax>(TInfo info, TSyntax old, Func<AssemblyVisitor, TInfo, TSyntax, TSyntax> method) where TSyntax : MemberDeclarationSyntax
                {
                    // Visit a member, and automatically update the type if needed
                    TSyntax @new = old;

                    foreach (AssemblyVisitor visitor in visitors)
                    {
                        @new = method(visitor, info, @new);

                        if (@new == null)
                            break;
                    }

                    if (@new == null)
                        type = type.WithMembers(type.Members.RemoveAt(index--));
                    else if (@new != old)
                        type = type.WithMembers(type.Members.Replace(old, @new));
                }

                for (; index < type.Members.Count; index++)
                {
                    MemberDeclarationSyntax member = type.Members[index];

                    if (member is FieldDeclarationSyntax field)
                        DoMember(typeInfo.GetDeclaredField(field.Declaration.Variables.First().Identifier.Text), field, (v, i, s) => v.Visit(i, s));
                    else if (member is PropertyDeclarationSyntax prop)
                        DoMember(typeInfo.GetDeclaredProperty(prop.Identifier.Text), prop, (v, i, s) => v.Visit(i, s));
                    else if (member is EventDeclarationSyntax @event)
                        DoMember(typeInfo.GetDeclaredEvent(@event.Identifier.Text), @event, (v, i, s) => v.Visit(i, s));
                    else if (member is DelegateDeclarationSyntax @delegate)
                        DoMember(typeInfo.GetDeclaredNestedType(@delegate.Identifier.Text), @delegate, (v, i, s) => v.Visit(i, s));
                    else if (member is TypeDeclarationSyntax nestedType)
                    {
                        TypeDeclarationSyntax newNestedType = DoType(typeInfo.GetDeclaredNestedType(nestedType.Identifier.Text), nestedType);

                        if (newNestedType == null)
                            type = type.WithMembers(type.Members.RemoveAt(index--));
                        else if (newNestedType != nestedType)
                            type = type.WithMembers(type.Members.Replace(nestedType, newNestedType));
                    }
                    else if (member is MethodDeclarationSyntax method)
                    {
                        MethodInfo methodInfo = typeInfo.GetDeclaredMethod(method.Identifier.Text);
                        MethodDeclarationSyntax newMethod = method;

                        foreach (AssemblyVisitor visitor in visitors)
                        {
                            newMethod = visitor.Visit(methodInfo, newMethod);

                            if (newMethod == null)
                                break;

                            // Parameters
                            foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
                            {
                                // TODO: Make sure parameters haven't changed
                                ParameterSyntax oldParameter = newMethod.ParameterList.Parameters[paramInfo.Position];
                                ParameterSyntax newParameter = visitor.Visit(paramInfo, oldParameter);

                                if (oldParameter != newParameter)
                                    newMethod = newMethod.WithParameterList(
                                        newMethod.ParameterList.WithParameters(
                                            newMethod.ParameterList.Parameters.Replace(oldParameter, newParameter)
                                        )
                                    );
                            }
                        }

                        if (newMethod == null)
                            type = type.WithMembers(type.Members.RemoveAt(index--));
                        else if (newMethod != method)
                            type = type.WithMembers(type.Members.Replace(method, newMethod));
                    }
                }

                return type;
            }

            NamespaceDeclarationSyntax DoNamespace(NamespaceDeclarationSyntax ns)
            {
                // Visit every declaration inside a namespace
                int index = 0;

                void WatchMember<T>(T old, T @new) where T : MemberDeclarationSyntax
                {
                    if (@new == null)
                        ns = ns.WithMembers(ns.Members.RemoveAt(index--));
                    else if (old != @new)
                        ns = ns.WithMembers(ns.Members.Replace(old, @new));
                }

                for (; index < ns.Members.Count; index++)
                {
                    MemberDeclarationSyntax member = ns.Members[index];

                    if (member is NamespaceDeclarationSyntax _ns)
                        WatchMember(_ns, DoNamespace(_ns));
                    else if (member is TypeDeclarationSyntax _type)
                        WatchMember(_type, DoType(_type.AsType(), _type));
                    else if (member is DelegateDeclarationSyntax _delegate)
                        WatchMember(_delegate, DoTopLevel(_delegate.AsType(), _delegate, (v, i, s) => v.Visit(i, s)));
                    else
                        throw new NotSupportedException();
                }

                return ns;
            }

            // Take care of namespaces, types, etc
            for (int index = 0; index < unit.Members.Count; index++)
            {
                void WatchUnitMember<T>(T old, T @new) where T : MemberDeclarationSyntax
                {
                    if (@new == null)
                        unit = unit.WithMembers(unit.Members.RemoveAt(index--));
                    else if (old != @new)
                        unit = unit.WithMembers(unit.Members.Replace(old, @new));
                }

                MemberDeclarationSyntax member = unit.Members[index];

                if (member is NamespaceDeclarationSyntax ns)
                    WatchUnitMember(ns, DoNamespace(ns));
                else if (member is TypeDeclarationSyntax type)
                    WatchUnitMember(type, DoType(type.AsType(), type));
                else
                    throw new NotSupportedException();
            }

            if (unit != root)
                (root as CompilationUnitSyntax).Replace(u => u.WithMembers(unit.Members));
        }
    }
}
