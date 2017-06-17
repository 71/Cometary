using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    /// <summary>
    /// Provides the ability to print IL code directly.
    /// </summary>
    public static class IL
    {
        /// <summary>
        /// A list of actions to execute on saving.
        /// </summary>
        internal static readonly List<Action<AssemblyDefinition>> Actions = new List<Action<AssemblyDefinition>>();

        /// <summary>
        /// A list of instructions emitted in the current method.
        /// </summary>
        internal static readonly List<Instruction> EmittedInstructions = new List<Instruction>();

        /// <summary>
        /// Gets the assembly definition of the
        /// assembly being processed.
        /// </summary>
        internal static AssemblyDefinition Assembly { get; set; }

        /// <summary>
        /// Gets the main module definition of the assembly being processed.
        /// </summary>
        internal static ModuleDefinition Module => Assembly.MainModule;

        #region Callbacks
        /// <summary>
        /// Registers the given <paramref name="action"/> to be executed
        /// when the assembly is saved.
        /// </summary>
        public static void Do(Action<AssemblyDefinition> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Actions.Add(action);
        }

        /// <summary>
        /// Registers the given <paramref name="action"/> to be executed with
        /// the given <paramref name="type"/> when the assembly is saved.
        /// </summary>
        public static void Do(Type type, Action<TypeDefinition> action)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Do(assembly => action(assembly.MainModule.GetType(type.FullName)));
        }

        /// <summary>
        /// Registers the given <paramref name="action"/> to be executed with
        /// the given <paramref name="type"/> when the assembly is saved.
        /// </summary>
        public static void Do(TypeInfo type, Action<TypeDefinition> action)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Do(assembly => action(assembly.MainModule.GetType(type.FullName)));
        }

        /// <summary>
        /// Registers the given <paramref name="action"/> to be executed with
        /// the given <paramref name="method"/> when the assembly is saved.
        /// </summary>
        public static void Do(MethodInfo method, Action<MethodDefinition> action)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            ParameterInfo[] parameters = method.GetParameters();

            bool IsMatchingMethod(MethodDefinition def)
            {
                return def.Name == method.Name
                    && def.Parameters.Count == parameters.Length
                    && def.Parameters.Select(x => x.ParameterType.Name).SequenceEqual(parameters.Select(x => x.ParameterType.Name));
            }

            Do(method.DeclaringType, type => action(type.Methods.First(IsMatchingMethod)));
        }

        /// <summary>
        /// Registers the given <paramref name="action"/> to be executed with
        /// the given <paramref name="field"/> when the assembly is saved.
        /// </summary>
        public static void Do(FieldInfo field, Action<FieldDefinition> action)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Do(field.DeclaringType, type => action(type.Fields.First(x => x.Name == field.Name)));
        }

        /// <summary>
        /// Registers the given <paramref name="action"/> to be executed with
        /// the given <paramref name="property"/> when the assembly is saved.
        /// </summary>
        public static void Do(PropertyInfo property, Action<PropertyDefinition> action)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Do(property.DeclaringType, type => action(type.Properties.First(x => x.Name == property.Name)));
        }
        #endregion

        #region Emitting
        private static readonly Func<OpCode, object, Instruction> CreateInstruction = typeof(Instruction)
            .GetRuntimeMethod(".ctor", new[] { typeof(OpCode), typeof(object) })
            .CreateDelegate(typeof(Func<OpCode, object, Instruction>)) as Func<OpCode, object, Instruction>;

        /// <summary>
        /// Emits the given <see cref="Instruction"/> as raw IL.
        /// <para>
        /// This method can only be called in a method marked with an <see cref="ILAttribute"/>.
        /// </para>
        /// </summary>
        public static void Emit(Instruction instruction)
        {
            if (instruction == null)
                throw new ArgumentNullException(nameof(instruction));

            EmittedInstructions.Add(instruction);
        }

        /// <summary>
        /// Emits the given <see cref="OpCode"/> and its optional <paramref name="operand"/>
        /// as raw IL.
        /// <para>
        /// This method can only be called in a method marked with an <see cref="ILAttribute"/>.
        /// </para>
        /// </summary>
        public static void Emit(OpCode opcode, object operand = null) => Emit(CreateInstruction(opcode, operand));

        /// <summary>
        /// Emits the given <paramref name="il"/> code as raw IL.
        /// <para>
        /// The given string must be constant.
        /// </para>
        /// </summary>
        public static void Emit(string il)
        {
            if (!Meta.Compiling)
                throw new InvalidOperationException("Cannot emit IL code during runtime.");

            EmittedInstructions.AddRange(new ILParser(il));
        }
        #endregion
    }
}
