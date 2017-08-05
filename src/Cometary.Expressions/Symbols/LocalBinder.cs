using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    internal class LocalBinder
    {
        // Yes, there is no consistency between ImmutableArray<ILocalSymbol> Locals
        // and ParameterExpression[] Variables, and we shoulda used a Dictionary instead.
        // However, in code, we only use immutable arrays if locals, and actual arrays of variables.
        // So no problem here.

        public LocalBinder Parent { get; }
        public bool IsRoot { get; }

        private LocalBinder(LocalBinder parent)
        {
            Parent = parent;
            IsRoot = parent != null;
        }

        public static LocalBinder Root(ImmutableArray<ILocalSymbol> locals, ParameterExpression[] variables)
        {
            return new BlockLocalBinder(null, locals, variables);
        }

        public LocalBinder Child(ImmutableArray<ILocalSymbol> locals, ParameterExpression[] variables)
        {
            return new BlockLocalBinder(this, locals, variables);
        }

        public LocalBinder Child(ILocalSymbol local, ParameterExpression variable)
        {
            return new SingleLocalBinder(this, local, variable);
        }

        public static LocalBinder Empty => new LocalBinder(null);

        public virtual ParameterExpression this[ILocalSymbol local] => throw new KeyNotFoundException("Cannot find matching variable.");

        private sealed class BlockLocalBinder : LocalBinder
        {
            private readonly ImmutableArray<ILocalSymbol> Locals;
            private readonly ParameterExpression[] Variables;

            internal BlockLocalBinder(LocalBinder binder, ImmutableArray<ILocalSymbol> locals, ParameterExpression[] variables) : base(binder)
            {
                Locals = locals;
                Variables = variables;
            }

            public override ParameterExpression this[ILocalSymbol local]
            {
                get
                {
                    int index = Locals.IndexOf(local);

                    if (index != -1)
                        return Variables[index];

                    return Parent[local];
                }
            }
        }

        private sealed class SingleLocalBinder : LocalBinder
        {
            private readonly ILocalSymbol Local;
            private readonly ParameterExpression Variable;

            internal SingleLocalBinder(LocalBinder binder, ILocalSymbol local, ParameterExpression variable) : base(binder)
            {
                Local = local;
                Variable = variable;
            }

            public override ParameterExpression this[ILocalSymbol local] => Local == local ? Variable : Parent[local];
        }
    }
}
