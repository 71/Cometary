using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mono.Cecil.Cil;

namespace Cometary
{
    /// <summary>
    /// Helper class used to parse an IL <see cref="string"/> into
    /// one or many instructions.
    /// </summary>
    internal sealed class ILParser : IEnumerator<Instruction>, IEnumerable<Instruction>
    {
        #region Static utils
        private static Lazy<Dictionary<string, OpCode>> OpCodesMap
            = new Lazy<Dictionary<string, OpCode>>(MakeOpCodesMap);

        private static Dictionary<string, OpCode> MakeOpCodesMap()
        {
            Dictionary<string, OpCode> map = new Dictionary<string, OpCode>(StringComparer.OrdinalIgnoreCase);

            foreach (FieldInfo field in typeof(OpCodes).GetRuntimeFields())
            {
                if (field.FieldType != typeof(OpCode))
                    continue;

                map[field.Name] = (OpCode)field.GetValue(null);
            }

            return map;
        }
        #endregion

        // The IL code to parse.
        public string IL { get; }

        // The current position in the code to parse.
        public int Position { get; private set; }

        // StringBuilder used to parse different opcode names.
        private readonly StringBuilder builder;

        /// <inheritdoc />
        public Instruction Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public IEnumerator<Instruction> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this;

        public ILParser(string il)
        {
            IL = il;
            Position = -1;

            builder = new StringBuilder();
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (Position >= IL.Length)
                return false;

            SkipWhitespaces();

            // Get opcode name
            int indexOfNextWs = IL.IndexOfAny(new[] { ' ' });
            string opName = indexOfNextWs == -1 ? IL.Substring(Position) : IL.Substring(Position, indexOfNextWs - Position);

            // Get matching OpCode
            string opNameStr = opName.Replace('.', '_');

            if (!OpCodesMap.Value.TryGetValue(opNameStr, out OpCode opCode))
                throw new ArgumentException("The given op code does not exist.");

            if (indexOfNextWs == -1)
            {
                Position = IL.Length - 1;
                Current = Instruction.Create(opCode);
                return true;
            }

            Position = indexOfNextWs;

            SkipWhitespaces();

            // Get operand name
            int indexOfEndStmt = IL.IndexOfAny(new[] { '\n', ';' }, Position);
            var operandName = indexOfEndStmt == -1 ? IL.Substring(Position) : IL.Substring(Position, indexOfEndStmt - Position);

            Position = indexOfEndStmt == -1 ? IL.Length : indexOfEndStmt;

            // Parse operand, ouch
            // TODO, heh.

            Current = Instruction.Create(opCode, operandName);
            return true;
        }

        private void SkipWhitespaces()
        {
            while (char.IsWhiteSpace(IL[++Position])) ;
        }

        /// <inheritdoc />
        public void Reset() => Position = -1;

        /// <inheritdoc />
        public void Dispose() { }
    }
}
