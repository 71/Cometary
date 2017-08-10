using System;
using System.Reflection.Metadata;

namespace Cometary
{
    partial class IL
    {
        /// <summary>
        ///   Emits an <see cref="ILOpCode.Ldtoken"/> opcode, followed by a token representing the specified type.
        /// </summary>
        public static void Ldtoken(Type type) => Emit(ILOpCode.Nop);

        /// <summary>
        ///   Emits an <see cref="ILOpCode.Ldtoken"/> opcode, followed by a token representing the specified method.
        /// </summary>
        public static void Ldtoken<TDelegate>(TDelegate method) where TDelegate : Delegate => Emit(ILOpCode.Nop);

        /// <summary>
        ///   Emits an <see cref="ILOpCode.Ldtoken"/> opcode, followed by a token representing the referenced member.
        /// </summary>
        public static void Ldtoken(object memberAccess) => Emit(ILOpCode.Nop);
    }
}
