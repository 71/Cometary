using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Cometary
{
    partial class IL
    {
        #region 
        /// <summary>
        ///   Add two values, returning a new value.
        /// </summary>
        public static void Add() => Emit(new Instruction(OpCodes.Add));
        
        /// <summary>
        ///   Add signed integer values with overflow check.
        /// </summary>
        public static void Add_Ovf() => Emit(new Instruction(OpCodes.Add_Ovf));
        
        /// <summary>
        ///   Add unsigned integer values with overflow check.
        /// </summary>
        public static void Add_Ovf_Un() => Emit(new Instruction(OpCodes.Add_Ovf_Un));
        
        /// <summary>
        ///   Bitwise AND of two integral values, returns an integral value.
        /// </summary>
        public static void And() => Emit(new Instruction(OpCodes.And));
        
        /// <summary>
        ///   Return argument list handle for the current method.
        /// </summary>
        public static void Arglist() => Emit(new Instruction(OpCodes.Arglist));
        
        /// <summary>
        ///   Inform a debugger that a breakpoint has been reached.
        /// </summary>
        public static void Break() => Emit(new Instruction(OpCodes.Break));
        
        /// <summary>
        ///   Push 1 (of type int32) if value1 equals value2, else push 0.
        /// </summary>
        public static void Ceq() => Emit(new Instruction(OpCodes.Ceq));
        
        /// <summary>
        ///   Push 1 (of type int32) if value1 &gt; value2, else push 0.
        /// </summary>
        public static void Cgt() => Emit(new Instruction(OpCodes.Cgt));
        
        /// <summary>
        ///   Push 1 (of type int32) if value1 &gt; value2, unsigned or unordered, else push 0.
        /// </summary>
        public static void Cgt_Un() => Emit(new Instruction(OpCodes.Cgt_Un));
        
        /// <summary>
        ///   Throw ArithmeticException if value is not a finite number.
        /// </summary>
        public static void Ckfinite() => Emit(new Instruction(OpCodes.Ckfinite));
        
        /// <summary>
        ///   Push 1 (of type int32) if value1 &lt; value2, else push 0.
        /// </summary>
        public static void Clt() => Emit(new Instruction(OpCodes.Clt));
        
        /// <summary>
        ///   Push 1 (of type int32) if value1 &lt; value2, unsigned or unordered, else push 0.
        /// </summary>
        public static void Clt_Un() => Emit(new Instruction(OpCodes.Clt_Un));
        
        /// <summary>
        ///   Convert to native int, pushing native int on stack.
        /// </summary>
        public static void Conv_I() => Emit(new Instruction(OpCodes.Conv_I));
        
        /// <summary>
        ///   Convert to int8, pushing int32 on stack.
        /// </summary>
        public static void Conv_I1() => Emit(new Instruction(OpCodes.Conv_I1));
        
        /// <summary>
        ///   Convert to int16, pushing int32 on stack.
        /// </summary>
        public static void Conv_I2() => Emit(new Instruction(OpCodes.Conv_I2));
        
        /// <summary>
        ///   Convert to int32, pushing int32 on stack.
        /// </summary>
        public static void Conv_I4() => Emit(new Instruction(OpCodes.Conv_I4));
        
        /// <summary>
        ///   Convert to int64, pushing int64 on stack.
        /// </summary>
        public static void Conv_I8() => Emit(new Instruction(OpCodes.Conv_I8));
        
        /// <summary>
        ///   Convert to a native int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I() => Emit(new Instruction(OpCodes.Conv_Ovf_I));
        
        /// <summary>
        ///   Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_I_Un));
        
        /// <summary>
        ///   Convert to an int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I1() => Emit(new Instruction(OpCodes.Conv_Ovf_I1));
        
        /// <summary>
        ///   Convert unsigned to an int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I1_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_I1_Un));
        
        /// <summary>
        ///   Convert to an int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I2() => Emit(new Instruction(OpCodes.Conv_Ovf_I2));
        
        /// <summary>
        ///   Convert unsigned to an int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I2_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_I2_Un));
        
        /// <summary>
        ///   Convert to an int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I4() => Emit(new Instruction(OpCodes.Conv_Ovf_I4));
        
        /// <summary>
        ///   Convert unsigned to an int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I4_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_I4_Un));
        
        /// <summary>
        ///   Convert to an int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I8() => Emit(new Instruction(OpCodes.Conv_Ovf_I8));
        
        /// <summary>
        ///   Convert unsigned to an int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_I8_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_I8_Un));
        
        /// <summary>
        ///   Convert to a native unsigned int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U() => Emit(new Instruction(OpCodes.Conv_Ovf_U));
        
        /// <summary>
        ///   Convert unsigned to a native unsigned int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_U_Un));
        
        /// <summary>
        ///   Convert to an unsigned int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U1() => Emit(new Instruction(OpCodes.Conv_Ovf_U1));
        
        /// <summary>
        ///   Convert unsigned to an unsigned int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U1_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_U1_Un));
        
        /// <summary>
        ///   Convert to an unsigned int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U2() => Emit(new Instruction(OpCodes.Conv_Ovf_U2));
        
        /// <summary>
        ///   Convert unsigned to an unsigned int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U2_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_U2_Un));
        
        /// <summary>
        ///   Convert to an unsigned int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U4() => Emit(new Instruction(OpCodes.Conv_Ovf_U4));
        
        /// <summary>
        ///   Convert unsigned to an unsigned int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U4_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_U4_Un));
        
        /// <summary>
        ///   Convert to an unsigned int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U8() => Emit(new Instruction(OpCodes.Conv_Ovf_U8));
        
        /// <summary>
        ///   Convert unsigned to an unsigned int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static void Conv_Ovf_U8_Un() => Emit(new Instruction(OpCodes.Conv_Ovf_U8_Un));
        
        /// <summary>
        ///   Convert unsigned integer to floating-point, pushing F on stack.
        /// </summary>
        public static void Conv_R_Un() => Emit(new Instruction(OpCodes.Conv_R_Un));
        
        /// <summary>
        ///   Convert to float32, pushing F on stack.
        /// </summary>
        public static void Conv_R4() => Emit(new Instruction(OpCodes.Conv_R4));
        
        /// <summary>
        ///   Convert to float64, pushing F on stack.
        /// </summary>
        public static void Conv_R8() => Emit(new Instruction(OpCodes.Conv_R8));
        
        /// <summary>
        ///   Convert to native unsigned int, pushing native int on stack.
        /// </summary>
        public static void Conv_U() => Emit(new Instruction(OpCodes.Conv_U));
        
        /// <summary>
        ///   Convert to unsigned int8, pushing int32 on stack.
        /// </summary>
        public static void Conv_U1() => Emit(new Instruction(OpCodes.Conv_U1));
        
        /// <summary>
        ///   Convert to unsigned int16, pushing int32 on stack.
        /// </summary>
        public static void Conv_U2() => Emit(new Instruction(OpCodes.Conv_U2));
        
        /// <summary>
        ///   Convert to unsigned int32, pushing int32 on stack.
        /// </summary>
        public static void Conv_U4() => Emit(new Instruction(OpCodes.Conv_U4));
        
        /// <summary>
        ///   Convert to unsigned int64, pushing int64 on stack.
        /// </summary>
        public static void Conv_U8() => Emit(new Instruction(OpCodes.Conv_U8));
        
        /// <summary>
        ///   Copy data from memory to memory.
        /// </summary>
        public static void Cpblk() => Emit(new Instruction(OpCodes.Cpblk));
        
        /// <summary>
        ///   Divide two values to return a quotient or floating-point result.
        /// </summary>
        public static void Div() => Emit(new Instruction(OpCodes.Div));
        
        /// <summary>
        ///   Divide two values, unsigned, returning a quotient.
        /// </summary>
        public static void Div_Un() => Emit(new Instruction(OpCodes.Div_Un));
        
        /// <summary>
        ///   Duplicate the value on the top of the stack.
        /// </summary>
        public static void Dup() => Emit(new Instruction(OpCodes.Dup));
        
        /// <summary>
        ///   End an exception handling filter clause.
        /// </summary>
        public static void Endfilter() => Emit(new Instruction(OpCodes.Endfilter));
        
        /// <summary>
        ///   End finally clause of an exception block.
        /// </summary>
        public static void Endfinally() => Emit(new Instruction(OpCodes.Endfinally));
        
        /// <summary>
        ///   Set all bytes in a block of memory to a given byte value.
        /// </summary>
        public static void Initblk() => Emit(new Instruction(OpCodes.Initblk));
        
        /// <summary>
        ///   Load argument 0 onto the stack.
        /// </summary>
        public static void Ldarg_0() => Emit(new Instruction(OpCodes.Ldarg_0));
        
        /// <summary>
        ///   Load argument 1 onto the stack.
        /// </summary>
        public static void Ldarg_1() => Emit(new Instruction(OpCodes.Ldarg_1));
        
        /// <summary>
        ///   Load argument 2 onto the stack.
        /// </summary>
        public static void Ldarg_2() => Emit(new Instruction(OpCodes.Ldarg_2));
        
        /// <summary>
        ///   Load argument 3 onto the stack.
        /// </summary>
        public static void Ldarg_3() => Emit(new Instruction(OpCodes.Ldarg_3));
        
        /// <summary>
        ///   Push 0 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_0() => Emit(new Instruction(OpCodes.Ldc_I4_0));
        
        /// <summary>
        ///   Push 1 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_1() => Emit(new Instruction(OpCodes.Ldc_I4_1));
        
        /// <summary>
        ///   Push 2 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_2() => Emit(new Instruction(OpCodes.Ldc_I4_2));
        
        /// <summary>
        ///   Push 3 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_3() => Emit(new Instruction(OpCodes.Ldc_I4_3));
        
        /// <summary>
        ///   Push 4 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_4() => Emit(new Instruction(OpCodes.Ldc_I4_4));
        
        /// <summary>
        ///   Push 5 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_5() => Emit(new Instruction(OpCodes.Ldc_I4_5));
        
        /// <summary>
        ///   Push 6 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_6() => Emit(new Instruction(OpCodes.Ldc_I4_6));
        
        /// <summary>
        ///   Push 7 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_7() => Emit(new Instruction(OpCodes.Ldc_I4_7));
        
        /// <summary>
        ///   Push 8 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_8() => Emit(new Instruction(OpCodes.Ldc_I4_8));
        
        /// <summary>
        ///   Push -1 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4_M1() => Emit(new Instruction(OpCodes.Ldc_I4_M1));
        
        /// <summary>
        ///   Indirect load value of type native int as native int on the stack
        /// </summary>
        public static void Ldind_I() => Emit(new Instruction(OpCodes.Ldind_I));
        
        /// <summary>
        ///   Indirect load value of type int8 as int32 on the stack.
        /// </summary>
        public static void Ldind_I1() => Emit(new Instruction(OpCodes.Ldind_I1));
        
        /// <summary>
        ///   Indirect load value of type int16 as int32 on the stack.
        /// </summary>
        public static void Ldind_I2() => Emit(new Instruction(OpCodes.Ldind_I2));
        
        /// <summary>
        ///   Indirect load value of type int32 as int32 on the stack.
        /// </summary>
        public static void Ldind_I4() => Emit(new Instruction(OpCodes.Ldind_I4));
        
        /// <summary>
        ///   Indirect load value of type int64 as int64 on the stack.
        /// </summary>
        public static void Ldind_I8() => Emit(new Instruction(OpCodes.Ldind_I8));
        
        /// <summary>
        ///   Indirect load value of type float32 as F on the stack.
        /// </summary>
        public static void Ldind_R4() => Emit(new Instruction(OpCodes.Ldind_R4));
        
        /// <summary>
        ///   Indirect load value of type float64 as F on the stack.
        /// </summary>
        public static void Ldind_R8() => Emit(new Instruction(OpCodes.Ldind_R8));
        
        /// <summary>
        ///   Indirect load value of type object ref as O on the stack.
        /// </summary>
        public static void Ldind_Ref() => Emit(new Instruction(OpCodes.Ldind_Ref));
        
        /// <summary>
        ///   Indirect load value of type unsigned int8 as int32 on the stack
        /// </summary>
        public static void Ldind_U1() => Emit(new Instruction(OpCodes.Ldind_U1));
        
        /// <summary>
        ///   Indirect load value of type unsigned int16 as int32 on the stack
        /// </summary>
        public static void Ldind_U2() => Emit(new Instruction(OpCodes.Ldind_U2));
        
        /// <summary>
        ///   Indirect load value of type unsigned int32 as int32 on the stack
        /// </summary>
        public static void Ldind_U4() => Emit(new Instruction(OpCodes.Ldind_U4));
        
        /// <summary>
        ///   Push the length (of type native unsigned int) of array on the stack.
        /// </summary>
        public static void Ldlen() => Emit(new Instruction(OpCodes.Ldlen));
        
        /// <summary>
        ///   Load local variable 0 onto stack.
        /// </summary>
        public static void Ldloc_0() => Emit(new Instruction(OpCodes.Ldloc_0));
        
        /// <summary>
        ///   Load local variable 1 onto stack.
        /// </summary>
        public static void Ldloc_1() => Emit(new Instruction(OpCodes.Ldloc_1));
        
        /// <summary>
        ///   Load local variable 2 onto stack.
        /// </summary>
        public static void Ldloc_2() => Emit(new Instruction(OpCodes.Ldloc_2));
        
        /// <summary>
        ///   Load local variable 3 onto stack.
        /// </summary>
        public static void Ldloc_3() => Emit(new Instruction(OpCodes.Ldloc_3));
        
        /// <summary>
        ///   Push a null reference on the stack.
        /// </summary>
        public static void Ldnull() => Emit(new Instruction(OpCodes.Ldnull));
        
        /// <summary>
        ///   Allocate space from the local memory pool.
        /// </summary>
        public static void Localloc() => Emit(new Instruction(OpCodes.Localloc));
        
        /// <summary>
        ///   Multiply values.
        /// </summary>
        public static void Mul() => Emit(new Instruction(OpCodes.Mul));
        
        /// <summary>
        ///   Multiply signed integer values. Signed result shall fit in same size
        /// </summary>
        public static void Mul_Ovf() => Emit(new Instruction(OpCodes.Mul_Ovf));
        
        /// <summary>
        ///   Multiply unsigned integer values. Unsigned result shall fit in same size
        /// </summary>
        public static void Mul_Ovf_Un() => Emit(new Instruction(OpCodes.Mul_Ovf_Un));
        
        /// <summary>
        ///   Negate value.
        /// </summary>
        public static void Neg() => Emit(new Instruction(OpCodes.Neg));
        
        /// <summary>
        ///   Do nothing (No operation).
        /// </summary>
        public static void Nop() => Emit(new Instruction(OpCodes.Nop));
        
        /// <summary>
        ///   Bitwise complement (logical not).
        /// </summary>
        public static void Not() => Emit(new Instruction(OpCodes.Not));
        
        /// <summary>
        ///   Bitwise OR of two integer values, returns an integer.
        /// </summary>
        public static void Or() => Emit(new Instruction(OpCodes.Or));
        
        /// <summary>
        ///   Pop value from the stack.
        /// </summary>
        public static void Pop() => Emit(new Instruction(OpCodes.Pop));
        
        /// <summary>
        ///   Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer
        /// </summary>
        public static void Readonly() => Emit(new Instruction(OpCodes.Readonly));
        
        /// <summary>
        ///   Push the type token stored in a typed reference.
        /// </summary>
        public static void Refanytype() => Emit(new Instruction(OpCodes.Refanytype));
        
        /// <summary>
        ///   Remainder when dividing one value by another.
        /// </summary>
        public static void Rem() => Emit(new Instruction(OpCodes.Rem));
        
        /// <summary>
        ///   Remainder when dividing one unsigned value by another.
        /// </summary>
        public static void Rem_Un() => Emit(new Instruction(OpCodes.Rem_Un));
        
        /// <summary>
        ///   Return from method, possibly with a value.
        /// </summary>
        public static void Ret() => Emit(new Instruction(OpCodes.Ret));
        
        /// <summary>
        ///   Rethrow the current exception.
        /// </summary>
        public static void Rethrow() => Emit(new Instruction(OpCodes.Rethrow));
        
        /// <summary>
        ///   Shift an integer left (shifting in zeros), return an integer.
        /// </summary>
        public static void Shl() => Emit(new Instruction(OpCodes.Shl));
        
        /// <summary>
        ///   Shift an integer right (shift in sign), return an integer.
        /// </summary>
        public static void Shr() => Emit(new Instruction(OpCodes.Shr));
        
        /// <summary>
        ///   Shift an integer right (shift in zero), return an integer.
        /// </summary>
        public static void Shr_Un() => Emit(new Instruction(OpCodes.Shr_Un));
        
        /// <summary>
        ///   Store value of type native int into memory at address
        /// </summary>
        public static void Stind_I() => Emit(new Instruction(OpCodes.Stind_I));
        
        /// <summary>
        ///   Store value of type int8 into memory at address
        /// </summary>
        public static void Stind_I1() => Emit(new Instruction(OpCodes.Stind_I1));
        
        /// <summary>
        ///   Store value of type int16 into memory at address
        /// </summary>
        public static void Stind_I2() => Emit(new Instruction(OpCodes.Stind_I2));
        
        /// <summary>
        ///   Store value of type int32 into memory at address
        /// </summary>
        public static void Stind_I4() => Emit(new Instruction(OpCodes.Stind_I4));
        
        /// <summary>
        ///   Store value of type int64 into memory at address
        /// </summary>
        public static void Stind_I8() => Emit(new Instruction(OpCodes.Stind_I8));
        
        /// <summary>
        ///   Store value of type float32 into memory at address
        /// </summary>
        public static void Stind_R4() => Emit(new Instruction(OpCodes.Stind_R4));
        
        /// <summary>
        ///   Store value of type float64 into memory at address
        /// </summary>
        public static void Stind_R8() => Emit(new Instruction(OpCodes.Stind_R8));
        
        /// <summary>
        ///   Store value of type object ref (type O) into memory at address
        /// </summary>
        public static void Stind_Ref() => Emit(new Instruction(OpCodes.Stind_Ref));
        
        /// <summary>
        ///   Pop a value from stack into local variable 0.
        /// </summary>
        public static void Stloc_0() => Emit(new Instruction(OpCodes.Stloc_0));
        
        /// <summary>
        ///   Pop a value from stack into local variable 1.
        /// </summary>
        public static void Stloc_1() => Emit(new Instruction(OpCodes.Stloc_1));
        
        /// <summary>
        ///   Pop a value from stack into local variable 2.
        /// </summary>
        public static void Stloc_2() => Emit(new Instruction(OpCodes.Stloc_2));
        
        /// <summary>
        ///   Pop a value from stack into local variable 3.
        /// </summary>
        public static void Stloc_3() => Emit(new Instruction(OpCodes.Stloc_3));
        
        /// <summary>
        ///   Subtract value2 from value1, returning a new value.
        /// </summary>
        public static void Sub() => Emit(new Instruction(OpCodes.Sub));
        
        /// <summary>
        ///   Subtract native int from a native int. Signed result shall fit in same size
        /// </summary>
        public static void Sub_Ovf() => Emit(new Instruction(OpCodes.Sub_Ovf));
        
        /// <summary>
        ///   Subtract native unsigned int from a native unsigned int. Unsigned result shall fit in same size.
        /// </summary>
        public static void Sub_Ovf_Un() => Emit(new Instruction(OpCodes.Sub_Ovf_Un));
        
        /// <summary>
        ///   Throw an exception.
        /// </summary>
        public static void Throw() => Emit(new Instruction(OpCodes.Throw));
        
        /// <summary>
        ///   Subsequent pointer reference is volatile.
        /// </summary>
        public static void Volatile() => Emit(new Instruction(OpCodes.Volatile));
        
        /// <summary>
        ///   Bitwise XOR of integer values, returns an integer.
        /// </summary>
        public static void Xor() => Emit(new Instruction(OpCodes.Xor));
        
        #endregion // 
        
        #region string target
        /// <summary>
        ///   Branch to target if equal.
        /// </summary>
        public static void Beq(string target) => Emit(new Instruction(OpCodes.Beq, ( target)));
        
        /// <summary>
        ///   Branch to target if equal, short form.
        /// </summary>
        public static void Beq_S(string target) => Emit(new Instruction(OpCodes.Beq_S, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than or equal to.
        /// </summary>
        public static void Bge(string target) => Emit(new Instruction(OpCodes.Bge, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than or equal to, short form.
        /// </summary>
        public static void Bge_S(string target) => Emit(new Instruction(OpCodes.Bge_S, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than or equal to (unsigned or unordered).
        /// </summary>
        public static void Bge_Un(string target) => Emit(new Instruction(OpCodes.Bge_Un, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than or equal to (unsigned or unordered), short form
        /// </summary>
        public static void Bge_Un_S(string target) => Emit(new Instruction(OpCodes.Bge_Un_S, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than.
        /// </summary>
        public static void Bgt(string target) => Emit(new Instruction(OpCodes.Bgt, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than, short form.
        /// </summary>
        public static void Bgt_S(string target) => Emit(new Instruction(OpCodes.Bgt_S, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than (unsigned or unordered).
        /// </summary>
        public static void Bgt_Un(string target) => Emit(new Instruction(OpCodes.Bgt_Un, ( target)));
        
        /// <summary>
        ///   Branch to target if greater than (unsigned or unordered), short form.
        /// </summary>
        public static void Bgt_Un_S(string target) => Emit(new Instruction(OpCodes.Bgt_Un_S, ( target)));
        
        /// <summary>
        ///   Branch to target if less than or equal to.
        /// </summary>
        public static void Ble(string target) => Emit(new Instruction(OpCodes.Ble, ( target)));
        
        /// <summary>
        ///   Branch to target if less than or equal to, short form.
        /// </summary>
        public static void Ble_S(string target) => Emit(new Instruction(OpCodes.Ble_S, ( target)));
        
        /// <summary>
        ///   Branch to target if less than or equal to (unsigned or unordered).
        /// </summary>
        public static void Ble_Un(string target) => Emit(new Instruction(OpCodes.Ble_Un, ( target)));
        
        /// <summary>
        ///   Branch to target if less than or equal to (unsigned or unordered), short form
        /// </summary>
        public static void Ble_Un_S(string target) => Emit(new Instruction(OpCodes.Ble_Un_S, ( target)));
        
        /// <summary>
        ///   Branch to target if less than.
        /// </summary>
        public static void Blt(string target) => Emit(new Instruction(OpCodes.Blt, ( target)));
        
        /// <summary>
        ///   Branch to target if less than, short form.
        /// </summary>
        public static void Blt_S(string target) => Emit(new Instruction(OpCodes.Blt_S, ( target)));
        
        /// <summary>
        ///   Branch to target if less than (unsigned or unordered).
        /// </summary>
        public static void Blt_Un(string target) => Emit(new Instruction(OpCodes.Blt_Un, ( target)));
        
        /// <summary>
        ///   Branch to target if less than (unsigned or unordered), short form.
        /// </summary>
        public static void Blt_Un_S(string target) => Emit(new Instruction(OpCodes.Blt_Un_S, ( target)));
        
        /// <summary>
        ///   Branch to target if unequal or unordered.
        /// </summary>
        public static void Bne_Un(string target) => Emit(new Instruction(OpCodes.Bne_Un, ( target)));
        
        /// <summary>
        ///   Branch to target if unequal or unordered, short form.
        /// </summary>
        public static void Bne_Un_S(string target) => Emit(new Instruction(OpCodes.Bne_Un_S, ( target)));
        
        /// <summary>
        ///   Branch to target.
        /// </summary>
        public static void Br(string target) => Emit(new Instruction(OpCodes.Br, ( target)));
        
        /// <summary>
        ///   Branch to target, short form.
        /// </summary>
        public static void Br_S(string target) => Emit(new Instruction(OpCodes.Br_S, ( target)));
        
        /// <summary>
        ///   Branch to target if value is zero (false).
        /// </summary>
        public static void Brfalse(string target) => Emit(new Instruction(OpCodes.Brfalse, ( target)));
        
        /// <summary>
        ///   Branch to target if value is zero (false), short form.
        /// </summary>
        public static void Brfalse_S(string target) => Emit(new Instruction(OpCodes.Brfalse_S, ( target)));
        
        /// <summary>
        ///   Branch to target if value is non-zero (true).
        /// </summary>
        public static void Brtrue(string target) => Emit(new Instruction(OpCodes.Brtrue, ( target)));
        
        /// <summary>
        ///   Branch to target if value is non-zero (true), short form.
        /// </summary>
        public static void Brtrue_S(string target) => Emit(new Instruction(OpCodes.Brtrue_S, ( target)));
        
        /// <summary>
        ///   Exit a protected region of code.
        /// </summary>
        public static void Leave(string target) => Emit(new Instruction(OpCodes.Leave, ( target)));
        
        /// <summary>
        ///   Exit a protected region of code, short form.
        /// </summary>
        public static void Leave_S(string target) => Emit(new Instruction(OpCodes.Leave_S, ( target)));
        
        #endregion // string target
        
        #region Type type
        /// <summary>
        ///   Convert a boxable value to its boxed form
        /// </summary>
        public static void Box(Type type) => Emit(new Instruction(OpCodes.Box, ( type)));
        
        /// <summary>
        ///   Cast obj to class.
        /// </summary>
        public static void Castclass(Type type) => Emit(new Instruction(OpCodes.Castclass, ( type)));
        
        /// <summary>
        ///   Call a virtual method on a type constrained to be type T
        /// </summary>
        public static void Constrained(Type type) => Emit(new Instruction(OpCodes.Constrained, ( type)));
        
        /// <summary>
        ///   Copy a value type from src to dest.
        /// </summary>
        public static void Cpobj(Type type) => Emit(new Instruction(OpCodes.Cpobj, ( type)));
        
        /// <summary>
        ///   Initialize the value at address dest.
        /// </summary>
        public static void Initobj(Type type) => Emit(new Instruction(OpCodes.Initobj, ( type)));
        
        /// <summary>
        ///   Test if obj is an instance of class, returning null or an instance of that class or interface.
        /// </summary>
        public static void Isinst(Type type) => Emit(new Instruction(OpCodes.Isinst, ( type)));
        
        /// <summary>
        ///   Copy the value stored at address src to the stack.
        /// </summary>
        public static void Ldobj(Type type) => Emit(new Instruction(OpCodes.Ldobj, ( type)));
        
        /// <summary>
        ///   Convert metadata token to its runtime representation.
        /// </summary>
        public static void Ldtoken(Type type) => Emit(new Instruction(OpCodes.Ldtoken, ( type)));
        
        /// <summary>
        ///   Push a typed reference to ptr of type class onto the stack.
        /// </summary>
        public static void Mkrefany(Type type) => Emit(new Instruction(OpCodes.Mkrefany, ( type)));
        
        /// <summary>
        ///   Push the address stored in a typed reference.
        /// </summary>
        public static void Refanyval(Type type) => Emit(new Instruction(OpCodes.Refanyval, ( type)));
        
        /// <summary>
        ///   Push the size, in bytes, of a type as an unsigned int32.
        /// </summary>
        public static void Sizeof(Type type) => Emit(new Instruction(OpCodes.Sizeof, ( type)));
        
        /// <summary>
        ///   Store a value of type typeTok at an address.
        /// </summary>
        public static void Stobj(Type type) => Emit(new Instruction(OpCodes.Stobj, ( type)));
        
        /// <summary>
        ///   Extract a value-type from obj, its boxed representation.
        /// </summary>
        public static void Unbox(Type type) => Emit(new Instruction(OpCodes.Unbox, ( type)));
        
        /// <summary>
        ///   Extract a value-type from obj, its boxed representation
        /// </summary>
        public static void Unbox_Any(Type type) => Emit(new Instruction(OpCodes.Unbox_Any, ( type)));
        
        #endregion // Type type
        
        #region MethodInfo method
        /// <summary>
        ///   Call method described by method.
        /// </summary>
        public static void Call(MethodInfo method) => Emit(new Instruction(OpCodes.Call, ( method)));
        
        /// <summary>
        ///   Call a method associated with an object.
        /// </summary>
        public static void Callvirt(MethodInfo method) => Emit(new Instruction(OpCodes.Callvirt, ( method)));
        
        /// <summary>
        ///   Exit current method and jump to the specified method.
        /// </summary>
        public static void Jmp(MethodInfo method) => Emit(new Instruction(OpCodes.Jmp, ( method)));
        
        /// <summary>
        ///   Push a pointer to a method referenced by method, on the stack.
        /// </summary>
        public static void Ldftn(MethodInfo method) => Emit(new Instruction(OpCodes.Ldftn, ( method)));
        
        /// <summary>
        ///   Convert metadata token to its runtime representation.
        /// </summary>
        public static void Ldtoken(MethodInfo method) => Emit(new Instruction(OpCodes.Ldtoken, ( method)));
        
        /// <summary>
        ///   Push address of virtual method on the stack.
        /// </summary>
        public static void Ldvirtftn(MethodInfo method) => Emit(new Instruction(OpCodes.Ldvirtftn, ( method)));
        
        #endregion // MethodInfo method
        
        #region CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes
        /// <summary>
        ///   Call method indicated on the stack with arguments described by callsitedescr.
        /// </summary>
        public static void Calli(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) => Emit(new Instruction(OpCodes.Calli, ( optionalParameterTypes)));
        
        #endregion // CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes
        
        #region byte nbr
        /// <summary>
        ///   Load argument numbered num onto the stack, short form.
        /// </summary>
        public static void Ldarg_S(byte num) => Emit(new Instruction(OpCodes.Ldarg_S, ( nbr)));
        
        /// <summary>
        ///   Fetch the address of argument argNum, short form.
        /// </summary>
        public static void Ldarga_S(byte argNum) => Emit(new Instruction(OpCodes.Ldarga_S, ( nbr)));
        
        /// <summary>
        ///   Load local variable of index indx onto stack, short form.
        /// </summary>
        public static void Ldloc_S(byte indx) => Emit(new Instruction(OpCodes.Ldloc_S, ( nbr)));
        
        /// <summary>
        ///   Load address of local variable with index indx, short form.
        /// </summary>
        public static void Ldloca_S(byte indx) => Emit(new Instruction(OpCodes.Ldloca_S, ( nbr)));
        
        /// <summary>
        ///   Store value to the argument numbered num, short form.
        /// </summary>
        public static void Starg_S(byte num) => Emit(new Instruction(OpCodes.Starg_S, ( nbr)));
        
        /// <summary>
        ///   Pop a value from stack into local variable indx, short form.
        /// </summary>
        public static void Stloc_S(byte indx) => Emit(new Instruction(OpCodes.Stloc_S, ( nbr)));
        
        #endregion // byte nbr
        
        #region int nbr
        /// <summary>
        ///   Push num of type int32 onto the stack as int32.
        /// </summary>
        public static void Ldc_I4(int num) => Emit(new Instruction(OpCodes.Ldc_I4, ( nbr)));
        
        #endregion // int nbr
        
        #region sbyte nbr
        /// <summary>
        ///   Push num onto the stack as int32, short form.
        /// </summary>
        public static void Ldc_I4_S(sbyte num) => Emit(new Instruction(OpCodes.Ldc_I4_S, ( nbr)));
        
        #endregion // sbyte nbr
        
        #region long nbr
        /// <summary>
        ///   Push num of type int64 onto the stack as int64.
        /// </summary>
        public static void Ldc_I8(long num) => Emit(new Instruction(OpCodes.Ldc_I8, ( nbr)));
        
        #endregion // long nbr
        
        #region float nbr
        /// <summary>
        ///   Push num of type float32 onto the stack as F.
        /// </summary>
        public static void Ldc_R4(float num) => Emit(new Instruction(OpCodes.Ldc_R4, ( nbr)));
        
        #endregion // float nbr
        
        #region double nbr
        /// <summary>
        ///   Push num of type float64 onto the stack as F.
        /// </summary>
        public static void Ldc_R8(double num) => Emit(new Instruction(OpCodes.Ldc_R8, ( nbr)));
        
        #endregion // double nbr
        
        #region FieldInfo field
        /// <summary>
        ///   Push the value of field of object (or value type) obj, onto the stack.
        /// </summary>
        public static void Ldfld(FieldInfo field) => Emit(new Instruction(OpCodes.Ldfld, ( field)));
        
        /// <summary>
        ///   Push the address of field of object obj on the stack.
        /// </summary>
        public static void Ldflda(FieldInfo field) => Emit(new Instruction(OpCodes.Ldflda, ( field)));
        
        /// <summary>
        ///   Push the value of field on the stack.
        /// </summary>
        public static void Ldsfld(FieldInfo field) => Emit(new Instruction(OpCodes.Ldsfld, ( field)));
        
        /// <summary>
        ///   Push the address of the static field, field, on the stack.
        /// </summary>
        public static void Ldsflda(FieldInfo field) => Emit(new Instruction(OpCodes.Ldsflda, ( field)));
        
        /// <summary>
        ///   Convert metadata token to its runtime representation.
        /// </summary>
        public static void Ldtoken(FieldInfo field) => Emit(new Instruction(OpCodes.Ldtoken, ( field)));
        
        /// <summary>
        ///   Replace the value of field of the object obj with value.
        /// </summary>
        public static void Stfld(FieldInfo field) => Emit(new Instruction(OpCodes.Stfld, ( field)));
        
        /// <summary>
        ///   Replace the value of field with val.
        /// </summary>
        public static void Stsfld(FieldInfo field) => Emit(new Instruction(OpCodes.Stsfld, ( field)));
        
        #endregion // FieldInfo field
        
        #region string str
        /// <summary>
        ///   Push a string object for the literal string.
        /// </summary>
        public static void Ldstr(string str) => Emit(new Instruction(OpCodes.Ldstr, ( str)));
        
        #endregion // string str
        
        #region Type itemType
        /// <summary>
        ///   Create a new array with elements of type etype.
        /// </summary>
        public static void Newarr(Type itemType) => Emit(new Instruction(OpCodes.Newarr, ( itemType)));
        
        #endregion // Type itemType
        
        #region ConstructorInfo ctor
        /// <summary>
        ///   Allocate an uninitialized object or value type and call ctor.
        /// </summary>
        public static void Newobj(ConstructorInfo ctor) => Emit(new Instruction(OpCodes.Newobj, ( ctor)));
        
        #endregion // ConstructorInfo ctor
        
        #region params string[] labelNames
        /// <summary>
        ///   Jump to one of n values.
        /// </summary>
        public static void Switch(params string[] labelNames) => Emit(new Instruction(OpCodes.Switch, ( labelNames)));
        
        #endregion // params string[] labelNames
        
    }
}
