using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Cometary.Extensions
{
    public static partial class CometaryExtensions
    {
        private static readonly Lazy<Func<Delegate, object, Delegate>> WithTargetCore;
        private static readonly Lazy<Action<SyntaxNode, int, int>> WithPositionAndWidthCore;
        private static readonly Lazy<Func<object, object>> CloneCore;

        static CometaryExtensions()
        {
            WithTargetCore = Helpers.MakeLazyDelegate<Func<Delegate, object, Delegate>>(nameof(WithTargetCore), il =>
            {
                // Clone delegate
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic));

                // Change the cloned delegate's target field
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, typeof(Delegate).GetField("_target", BindingFlags.Instance | BindingFlags.NonPublic));

                // Return the cloned delegate
                il.Emit(OpCodes.Ret);
            });

            WithPositionAndWidthCore = Helpers.MakeLazyDelegate<Action<SyntaxNode, int, int>>(nameof(WithPositionAndWidthCore), il =>
            {
                MethodInfo cloneMethod = typeof(object).GetMethod(nameof(MemberwiseClone), BindingFlags.Instance | BindingFlags.NonPublic);

                FieldInfo positionBackingField = typeof(SyntaxNode).GetField("<Position>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo greenBackingField = typeof(SyntaxNode).GetField("<Green>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

                PropertyInfo fullWidthProp = greenBackingField.FieldType.GetProperty("FullWidth", BindingFlags.Instance | BindingFlags.Public);

                // Change position
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, positionBackingField);

                // Get green node...
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldfld, greenBackingField);

                // ... clone it
                il.Emit(OpCodes.Callvirt, cloneMethod);
                il.Emit(OpCodes.Dup);

                // ... and change its fullwidth
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Callvirt, fullWidthProp.SetMethod);

                // ... and set the node's green node field to the new green node
                il.Emit(OpCodes.Stfld, greenBackingField);

                // Return
                il.Emit(OpCodes.Ret);
            });

            CloneCore = Helpers.MakeLazyDelegate<Func<object, object>>(nameof(CloneCore), il =>
            {
                MethodInfo method = typeof(object).GetMethod(nameof(MemberwiseClone), BindingFlags.Instance | BindingFlags.NonPublic);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, method);
                il.Emit(OpCodes.Ret);
            });
        }

        internal static T Clone<T>(this T obj) where T : class
        {
            return (T)CloneCore.Value(obj);
        }

        /// <summary>
        ///   Returns a copy of the given <see cref="SyntaxTree"/>, with its root changed.
        /// </summary>
        public static SyntaxTree WithRoot(this SyntaxTree syntaxTree, SyntaxNode root)
        {
            return syntaxTree.WithRootAndOptions(root, syntaxTree.Options);
        }

        /// <summary>
        ///   Returns a copy of the given <see cref="SyntaxTree"/>, with its options changed.
        /// </summary>
        public static SyntaxTree WithOptions(this SyntaxTree syntaxTree, ParseOptions options)
        {
            return syntaxTree.WithRootAndOptions(syntaxTree.GetRoot(), options);
        }

        /// <summary>
        ///   Returns a copy of the given <see cref="SyntaxTree"/>, with its options compatible
        ///   with Cometary's required options.
        /// </summary>
        public static SyntaxTree WithCometaryOptions(this CSharpSyntaxTree syntaxTree, CompilationEditor editor)
        {
            return syntaxTree.WithOptions(editor.GetRecomputationPipeline().MakeDelegate(opts => opts)(syntaxTree.Options));
        }

        /// <summary>
        ///   Returns a clone of the specified <paramref name="delegate"/>,
        ///   with its target changed.
        /// </summary>
        public static Delegate WithTarget(this Delegate @delegate, object target)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return WithTargetCore.Value(@delegate, target);
        }

        /// <summary>
        ///   Returns a clone of the specified <paramref name="node"/>, with its
        ///   position and width changed to the given values.
        /// </summary>
        public static T WithPositionAndWidth<T>(this T node, int position, int width) where T : SyntaxNode
        {
            T clone = node.Clone();

            WithPositionAndWidthCore.Value(clone, position, width);

            return clone;
        }

        /// <summary>
        ///   Returns a clone of the specified <paramref name="node"/>, with its
        ///   <see cref="SyntaxNode.Span"/> changed to the given value.
        /// </summary>
        /// <remarks>
        ///   The resulting <see cref="SyntaxNode.Span"/> may not be exactly equal to the given
        /// </remarks>
        public static T WithSpan<T>(this T node, TextSpan span) where T : SyntaxNode
        {
            return node.WithPositionAndWidth(span.Start, span.Length);
        }
    }
}
