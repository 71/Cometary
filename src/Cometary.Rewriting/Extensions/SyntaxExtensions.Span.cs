using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Cometary.Extensions
{
    using X = System.Linq.Expressions.Expression;

    partial class SyntaxExtensions
    {
        private const string PositionBackingFieldName  = "<Position>k__BackingField";
        private const string FullWidthBackingFieldName = "_fullWidth";

        private static Func<SyntaxNode, object> GetGreenNodeCore;
        private static Func<SyntaxToken, object> GetGreenTokenCore;
        private static Func<object, object> GetMemberwiseCloneCore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object GetGreenNode(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (GetGreenNodeCore == null)
            {
                var parameter = X.Parameter(typeof(SyntaxNode));

                GetGreenNodeCore = X
                    .Lambda<Func<SyntaxNode, object>>(X.Property(parameter, "Green"), parameter)
                    .Compile();
            }

            return GetGreenNodeCore(node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object GetGreenNode(SyntaxToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (GetGreenTokenCore == null)
            {
                var parameter = X.Parameter(typeof(SyntaxToken));

                GetGreenTokenCore = X
                    .Lambda<Func<SyntaxToken, object>>(X.Property(parameter, "Node"), parameter)
                    .Compile();
            }

            return GetGreenTokenCore(token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T GetMemberwiseClone<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (GetMemberwiseCloneCore == null)
            {
                var parameter = X.Parameter(typeof(object));

                GetMemberwiseCloneCore = X
                    .Lambda<Func<object, object>>(X.Call(parameter, typeof(object).GetRuntimeMethod(nameof(MemberwiseClone), Type.EmptyTypes)))
                    .Compile();
            }

            return (T)GetMemberwiseCloneCore(obj);
        }

        /// <summary>
        /// Returns a copy of the given <paramref name="node"/>, with a new <paramref name="position"/>.
        /// </summary>
        //[CopyTo("WithStart")]
        public static T WithPosition<T>(this T node, int position) where T : SyntaxNode
        {
            T copy = GetMemberwiseClone(node);

            typeof(SyntaxNode).GetRuntimeField(PositionBackingFieldName).SetValue(copy, position);

            return copy;
        }

        /// <summary>
        /// Returns a copy of the given <paramref name="node"/>, with a new full width.
        /// <para>
        /// Please note that the underlying green node is a reference type,
        /// which means that both the given <paramref name="node"/> and the
        /// returned node will share their new width.
        /// </para>
        /// </summary>
        public static T WithWidth<T>(this T node, int fullWidth) where T : SyntaxNode
        {
            T copy = GetMemberwiseClone(node);
            object greenNode = GetGreenNode(copy);

            greenNode.GetType().GetRuntimeField(FullWidthBackingFieldName).SetValue(greenNode, fullWidth);

            return copy;
        }

        /// <summary>
        /// Returns a copy of the given <paramref name="node"/>, with a new full <paramref name="span"/>.
        /// </summary>
        public static T WithSpan<T>(this T node, TextSpan span) where T : SyntaxNode
        {
            return node.WithPosition(span.Start).WithWidth(span.Length);
        }

        /// <summary>
        /// Returns a copy of the given <paramref name="node"/>, with a span copied from the <paramref name="other"/> node.
        /// </summary>
        public static T WithSpan<T>(this T node, SyntaxNode other) where T : SyntaxNode
        {
            return node.WithSpan(other.FullSpan);
        }


        /// <summary>
        /// Returns a copy of the given <paramref name="token"/>, with a new <paramref name="position"/>.
        /// </summary>
        //[CopyTo("WithStart")]
        public static SyntaxToken WithPosition(this SyntaxToken token, int position)
        {
            SyntaxToken copy = token;

            typeof(SyntaxToken).GetRuntimeField(PositionBackingFieldName).SetValue(copy, position);

            return copy;
        }

        /// <summary>
        /// Returns a copy of the given <paramref name="token"/>, with a new full width.
        /// <para>
        /// Please note that the underlying green node is a reference type,
        /// which means that both the given <paramref name="token"/> and the
        /// returned node will share their new width.
        /// </para>
        /// </summary>
        public static SyntaxToken WithWidth(this SyntaxToken token, int fullWidth)
        {
            SyntaxToken copy = token;
            object greenNode = GetGreenNode(copy);

            greenNode.GetType().GetRuntimeField(FullWidthBackingFieldName).SetValue(greenNode, fullWidth);

            return copy;
        }

        /// <summary>
        /// Returns a copy of the given <paramref name="token"/>, with a new full <paramref name="span"/>.
        /// </summary>
        public static SyntaxToken WithSpan(this SyntaxToken token, TextSpan span)
        {
            return token.WithPosition(span.Start).WithWidth(span.Length);
        }

        /// <summary>
        /// Returns a copy of the given <paramref name="token"/>, with a span copied from the <paramref name="other"/> token.
        /// </summary>
        public static SyntaxToken WithSpan(this SyntaxToken token, SyntaxToken other)
        {
            return token.WithSpan(other.FullSpan);
        }
    }
}
