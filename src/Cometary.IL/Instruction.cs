using System.Reflection.Emit;

namespace Cometary
{
    /// <summary>
    ///   Represents an IL instruction.
    /// </summary>
    public struct Instruction
    {
        /// <summary>
        ///   Gets the <see cref="System.Reflection.Emit.OpCode"/> to emit.
        /// </summary>
        public OpCode OpCode { get; }

        /// <summary>
        ///   Gets the boxed operand.
        /// </summary>
        public object Operand { get; }

        /// <summary>
        ///   Creates an <see cref="Instruction"/>, given its opcode and operand.
        /// </summary>
        internal Instruction(OpCode opCode, object operand = null)
        {
            OpCode = opCode;
            Operand = operand;
        }
    }
}