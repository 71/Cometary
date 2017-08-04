using System.Reflection.Emit;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    public struct Instruction
    {
        /// <summary>
        /// 
        /// </summary>
        public OpCode OpCode { get; }

        /// <summary>
        /// 
        /// </summary>
        public object Operand { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opCode"></param>
        internal Instruction(OpCode opCode, object operand = null)
        {
            OpCode = opCode;
            Operand = operand;
        }
    }
}
