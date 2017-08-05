using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.CodeAnalysis;

namespace Cometary
{
    partial class IL
    {
        #region No parameters
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Add"/>:</para>
        ///   <para>"Add two values, returning a new value."</para>
        /// </summary>
        public static void Add() => Emit(OpCodes.Add);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Add_Ovf"/>:</para>
        ///   <para>"Add signed integer values with overflow check."</para>
        /// </summary>
        public static void Add_Ovf() => Emit(OpCodes.Add_Ovf);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Add_Ovf_Un"/>:</para>
        ///   <para>"Add unsigned integer values with overflow check."</para>
        /// </summary>
        public static void Add_Ovf_Un() => Emit(OpCodes.Add_Ovf_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.And"/>:</para>
        ///   <para>"Bitwise AND of two integral values, returns an integral value."</para>
        /// </summary>
        public static void And() => Emit(OpCodes.And);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Arglist"/>:</para>
        ///   <para>"Return argument list handle for the current method."</para>
        /// </summary>
        public static void Arglist() => Emit(OpCodes.Arglist);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Break"/>:</para>
        ///   <para>"Inform a debugger that a breakpoint has been reached."</para>
        /// </summary>
        public static void Break() => Emit(OpCodes.Break);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ceq"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 equals value2, else push 0."</para>
        /// </summary>
        public static void Ceq() => Emit(OpCodes.Ceq);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cgt"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &gt; value2, else push 0."</para>
        /// </summary>
        public static void Cgt() => Emit(OpCodes.Cgt);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cgt_Un"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &gt; value2, unsigned or unordered, else push 0."</para>
        /// </summary>
        public static void Cgt_Un() => Emit(OpCodes.Cgt_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ckfinite"/>:</para>
        ///   <para>"Throw ArithmeticException if value is not a finite number."</para>
        /// </summary>
        public static void Ckfinite() => Emit(OpCodes.Ckfinite);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Clt"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &lt; value2, else push 0."</para>
        /// </summary>
        public static void Clt() => Emit(OpCodes.Clt);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Clt_Un"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &lt; value2, unsigned or unordered, else push 0."</para>
        /// </summary>
        public static void Clt_Un() => Emit(OpCodes.Clt_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I"/>:</para>
        ///   <para>"Convert to native int, pushing native int on stack."</para>
        /// </summary>
        public static void Conv_I() => Emit(OpCodes.Conv_I);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I1"/>:</para>
        ///   <para>"Convert to int8, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_I1() => Emit(OpCodes.Conv_I1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I2"/>:</para>
        ///   <para>"Convert to int16, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_I2() => Emit(OpCodes.Conv_I2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I4"/>:</para>
        ///   <para>"Convert to int32, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_I4() => Emit(OpCodes.Conv_I4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I8"/>:</para>
        ///   <para>"Convert to int64, pushing int64 on stack."</para>
        /// </summary>
        public static void Conv_I8() => Emit(OpCodes.Conv_I8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I"/>:</para>
        ///   <para>"Convert to a native int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I() => Emit(OpCodes.Conv_Ovf_I);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I_Un"/>:</para>
        ///   <para>"Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I_Un() => Emit(OpCodes.Conv_Ovf_I_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I1"/>:</para>
        ///   <para>"Convert to an int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I1() => Emit(OpCodes.Conv_Ovf_I1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I1_Un"/>:</para>
        ///   <para>"Convert unsigned to an int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I1_Un() => Emit(OpCodes.Conv_Ovf_I1_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I2"/>:</para>
        ///   <para>"Convert to an int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I2() => Emit(OpCodes.Conv_Ovf_I2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I2_Un"/>:</para>
        ///   <para>"Convert unsigned to an int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I2_Un() => Emit(OpCodes.Conv_Ovf_I2_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I4"/>:</para>
        ///   <para>"Convert to an int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I4() => Emit(OpCodes.Conv_Ovf_I4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I4_Un"/>:</para>
        ///   <para>"Convert unsigned to an int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I4_Un() => Emit(OpCodes.Conv_Ovf_I4_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I8"/>:</para>
        ///   <para>"Convert to an int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I8() => Emit(OpCodes.Conv_Ovf_I8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I8_Un"/>:</para>
        ///   <para>"Convert unsigned to an int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I8_Un() => Emit(OpCodes.Conv_Ovf_I8_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U"/>:</para>
        ///   <para>"Convert to a native unsigned int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U() => Emit(OpCodes.Conv_Ovf_U);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U_Un"/>:</para>
        ///   <para>"Convert unsigned to a native unsigned int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U_Un() => Emit(OpCodes.Conv_Ovf_U_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U1"/>:</para>
        ///   <para>"Convert to an unsigned int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U1() => Emit(OpCodes.Conv_Ovf_U1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U1_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U1_Un() => Emit(OpCodes.Conv_Ovf_U1_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U2"/>:</para>
        ///   <para>"Convert to an unsigned int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U2() => Emit(OpCodes.Conv_Ovf_U2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U2_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U2_Un() => Emit(OpCodes.Conv_Ovf_U2_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U4"/>:</para>
        ///   <para>"Convert to an unsigned int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U4() => Emit(OpCodes.Conv_Ovf_U4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U4_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U4_Un() => Emit(OpCodes.Conv_Ovf_U4_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U8"/>:</para>
        ///   <para>"Convert to an unsigned int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U8() => Emit(OpCodes.Conv_Ovf_U8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U8_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U8_Un() => Emit(OpCodes.Conv_Ovf_U8_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_R_Un"/>:</para>
        ///   <para>"Convert unsigned integer to floating-point, pushing F on stack."</para>
        /// </summary>
        public static void Conv_R_Un() => Emit(OpCodes.Conv_R_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_R4"/>:</para>
        ///   <para>"Convert to float32, pushing F on stack."</para>
        /// </summary>
        public static void Conv_R4() => Emit(OpCodes.Conv_R4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_R8"/>:</para>
        ///   <para>"Convert to float64, pushing F on stack."</para>
        /// </summary>
        public static void Conv_R8() => Emit(OpCodes.Conv_R8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U"/>:</para>
        ///   <para>"Convert to native unsigned int, pushing native int on stack."</para>
        /// </summary>
        public static void Conv_U() => Emit(OpCodes.Conv_U);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U1"/>:</para>
        ///   <para>"Convert to unsigned int8, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_U1() => Emit(OpCodes.Conv_U1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U2"/>:</para>
        ///   <para>"Convert to unsigned int16, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_U2() => Emit(OpCodes.Conv_U2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U4"/>:</para>
        ///   <para>"Convert to unsigned int32, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_U4() => Emit(OpCodes.Conv_U4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U8"/>:</para>
        ///   <para>"Convert to unsigned int64, pushing int64 on stack."</para>
        /// </summary>
        public static void Conv_U8() => Emit(OpCodes.Conv_U8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cpblk"/>:</para>
        ///   <para>"Copy data from memory to memory."</para>
        /// </summary>
        public static void Cpblk() => Emit(OpCodes.Cpblk);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Div"/>:</para>
        ///   <para>"Divide two values to return a quotient or floating-point result."</para>
        /// </summary>
        public static void Div() => Emit(OpCodes.Div);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Div_Un"/>:</para>
        ///   <para>"Divide two values, unsigned, returning a quotient."</para>
        /// </summary>
        public static void Div_Un() => Emit(OpCodes.Div_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Dup"/>:</para>
        ///   <para>"Duplicate the value on the top of the stack."</para>
        /// </summary>
        public static void Dup() => Emit(OpCodes.Dup);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Endfilter"/>:</para>
        ///   <para>"End an exception handling filter clause."</para>
        /// </summary>
        public static void Endfilter() => Emit(OpCodes.Endfilter);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Endfinally"/>:</para>
        ///   <para>"End finally clause of an exception block."</para>
        /// </summary>
        public static void Endfinally() => Emit(OpCodes.Endfinally);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Initblk"/>:</para>
        ///   <para>"Set all bytes in a block of memory to a given byte value."</para>
        /// </summary>
        public static void Initblk() => Emit(OpCodes.Initblk);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_0"/>:</para>
        ///   <para>"Load argument 0 onto the stack."</para>
        /// </summary>
        public static void Ldarg_0() => Emit(OpCodes.Ldarg_0);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_1"/>:</para>
        ///   <para>"Load argument 1 onto the stack."</para>
        /// </summary>
        public static void Ldarg_1() => Emit(OpCodes.Ldarg_1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_2"/>:</para>
        ///   <para>"Load argument 2 onto the stack."</para>
        /// </summary>
        public static void Ldarg_2() => Emit(OpCodes.Ldarg_2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_3"/>:</para>
        ///   <para>"Load argument 3 onto the stack."</para>
        /// </summary>
        public static void Ldarg_3() => Emit(OpCodes.Ldarg_3);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_0"/>:</para>
        ///   <para>"Push 0 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_0() => Emit(OpCodes.Ldc_I4_0);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_1"/>:</para>
        ///   <para>"Push 1 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_1() => Emit(OpCodes.Ldc_I4_1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_2"/>:</para>
        ///   <para>"Push 2 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_2() => Emit(OpCodes.Ldc_I4_2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_3"/>:</para>
        ///   <para>"Push 3 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_3() => Emit(OpCodes.Ldc_I4_3);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_4"/>:</para>
        ///   <para>"Push 4 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_4() => Emit(OpCodes.Ldc_I4_4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_5"/>:</para>
        ///   <para>"Push 5 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_5() => Emit(OpCodes.Ldc_I4_5);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_6"/>:</para>
        ///   <para>"Push 6 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_6() => Emit(OpCodes.Ldc_I4_6);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_7"/>:</para>
        ///   <para>"Push 7 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_7() => Emit(OpCodes.Ldc_I4_7);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_8"/>:</para>
        ///   <para>"Push 8 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_8() => Emit(OpCodes.Ldc_I4_8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_M1"/>:</para>
        ///   <para>"Push -1 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_M1() => Emit(OpCodes.Ldc_I4_M1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I"/>:</para>
        ///   <para>"Indirect load value of type native int as native int on the stack"</para>
        /// </summary>
        public static void Ldind_I() => Emit(OpCodes.Ldind_I);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I1"/>:</para>
        ///   <para>"Indirect load value of type int8 as int32 on the stack."</para>
        /// </summary>
        public static void Ldind_I1() => Emit(OpCodes.Ldind_I1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I2"/>:</para>
        ///   <para>"Indirect load value of type int16 as int32 on the stack."</para>
        /// </summary>
        public static void Ldind_I2() => Emit(OpCodes.Ldind_I2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I4"/>:</para>
        ///   <para>"Indirect load value of type int32 as int32 on the stack."</para>
        /// </summary>
        public static void Ldind_I4() => Emit(OpCodes.Ldind_I4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I8"/>:</para>
        ///   <para>"Indirect load value of type int64 as int64 on the stack."</para>
        /// </summary>
        public static void Ldind_I8() => Emit(OpCodes.Ldind_I8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_R4"/>:</para>
        ///   <para>"Indirect load value of type float32 as F on the stack."</para>
        /// </summary>
        public static void Ldind_R4() => Emit(OpCodes.Ldind_R4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_R8"/>:</para>
        ///   <para>"Indirect load value of type float64 as F on the stack."</para>
        /// </summary>
        public static void Ldind_R8() => Emit(OpCodes.Ldind_R8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_Ref"/>:</para>
        ///   <para>"Indirect load value of type object ref as O on the stack."</para>
        /// </summary>
        public static void Ldind_Ref() => Emit(OpCodes.Ldind_Ref);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_U1"/>:</para>
        ///   <para>"Indirect load value of type unsigned int8 as int32 on the stack"</para>
        /// </summary>
        public static void Ldind_U1() => Emit(OpCodes.Ldind_U1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_U2"/>:</para>
        ///   <para>"Indirect load value of type unsigned int16 as int32 on the stack"</para>
        /// </summary>
        public static void Ldind_U2() => Emit(OpCodes.Ldind_U2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_U4"/>:</para>
        ///   <para>"Indirect load value of type unsigned int32 as int32 on the stack"</para>
        /// </summary>
        public static void Ldind_U4() => Emit(OpCodes.Ldind_U4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldlen"/>:</para>
        ///   <para>"Push the length (of type native unsigned int) of array on the stack."</para>
        /// </summary>
        public static void Ldlen() => Emit(OpCodes.Ldlen);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_0"/>:</para>
        ///   <para>"Load local variable 0 onto stack."</para>
        /// </summary>
        public static void Ldloc_0() => Emit(OpCodes.Ldloc_0);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_1"/>:</para>
        ///   <para>"Load local variable 1 onto stack."</para>
        /// </summary>
        public static void Ldloc_1() => Emit(OpCodes.Ldloc_1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_2"/>:</para>
        ///   <para>"Load local variable 2 onto stack."</para>
        /// </summary>
        public static void Ldloc_2() => Emit(OpCodes.Ldloc_2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_3"/>:</para>
        ///   <para>"Load local variable 3 onto stack."</para>
        /// </summary>
        public static void Ldloc_3() => Emit(OpCodes.Ldloc_3);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldnull"/>:</para>
        ///   <para>"Push a null reference on the stack."</para>
        /// </summary>
        public static void Ldnull() => Emit(OpCodes.Ldnull);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Localloc"/>:</para>
        ///   <para>"Allocate space from the local memory pool."</para>
        /// </summary>
        public static void Localloc() => Emit(OpCodes.Localloc);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mul"/>:</para>
        ///   <para>"Multiply values."</para>
        /// </summary>
        public static void Mul() => Emit(OpCodes.Mul);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mul_Ovf"/>:</para>
        ///   <para>"Multiply signed integer values. Signed result shall fit in same size"</para>
        /// </summary>
        public static void Mul_Ovf() => Emit(OpCodes.Mul_Ovf);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mul_Ovf_Un"/>:</para>
        ///   <para>"Multiply unsigned integer values. Unsigned result shall fit in same size"</para>
        /// </summary>
        public static void Mul_Ovf_Un() => Emit(OpCodes.Mul_Ovf_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Neg"/>:</para>
        ///   <para>"Negate value."</para>
        /// </summary>
        public static void Neg() => Emit(OpCodes.Neg);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Nop"/>:</para>
        ///   <para>"Do nothing (No operation)."</para>
        /// </summary>
        public static void Nop() => Emit(OpCodes.Nop);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Not"/>:</para>
        ///   <para>"Bitwise complement (logical not)."</para>
        /// </summary>
        public static void Not() => Emit(OpCodes.Not);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Or"/>:</para>
        ///   <para>"Bitwise OR of two integer values, returns an integer."</para>
        /// </summary>
        public static void Or() => Emit(OpCodes.Or);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Pop"/>:</para>
        ///   <para>"Pop value from the stack."</para>
        /// </summary>
        public static void Pop() => Emit(OpCodes.Pop);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Readonly"/>:</para>
        ///   <para>"Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer"</para>
        /// </summary>
        public static void Readonly() => Emit(OpCodes.Readonly);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Refanytype"/>:</para>
        ///   <para>"Push the type token stored in a typed reference."</para>
        /// </summary>
        public static void Refanytype() => Emit(OpCodes.Refanytype);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Rem"/>:</para>
        ///   <para>"Remainder when dividing one value by another."</para>
        /// </summary>
        public static void Rem() => Emit(OpCodes.Rem);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Rem_Un"/>:</para>
        ///   <para>"Remainder when dividing one unsigned value by another."</para>
        /// </summary>
        public static void Rem_Un() => Emit(OpCodes.Rem_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ret"/>:</para>
        ///   <para>"Return from method, possibly with a value."</para>
        /// </summary>
        public static void Ret() => Emit(OpCodes.Ret);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Rethrow"/>:</para>
        ///   <para>"Rethrow the current exception."</para>
        /// </summary>
        public static void Rethrow() => Emit(OpCodes.Rethrow);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Shl"/>:</para>
        ///   <para>"Shift an integer left (shifting in zeros), return an integer."</para>
        /// </summary>
        public static void Shl() => Emit(OpCodes.Shl);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Shr"/>:</para>
        ///   <para>"Shift an integer right (shift in sign), return an integer."</para>
        /// </summary>
        public static void Shr() => Emit(OpCodes.Shr);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Shr_Un"/>:</para>
        ///   <para>"Shift an integer right (shift in zero), return an integer."</para>
        /// </summary>
        public static void Shr_Un() => Emit(OpCodes.Shr_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I"/>:</para>
        ///   <para>"Store value of type native int into memory at address"</para>
        /// </summary>
        public static void Stind_I() => Emit(OpCodes.Stind_I);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I1"/>:</para>
        ///   <para>"Store value of type int8 into memory at address"</para>
        /// </summary>
        public static void Stind_I1() => Emit(OpCodes.Stind_I1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I2"/>:</para>
        ///   <para>"Store value of type int16 into memory at address"</para>
        /// </summary>
        public static void Stind_I2() => Emit(OpCodes.Stind_I2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I4"/>:</para>
        ///   <para>"Store value of type int32 into memory at address"</para>
        /// </summary>
        public static void Stind_I4() => Emit(OpCodes.Stind_I4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I8"/>:</para>
        ///   <para>"Store value of type int64 into memory at address"</para>
        /// </summary>
        public static void Stind_I8() => Emit(OpCodes.Stind_I8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_R4"/>:</para>
        ///   <para>"Store value of type float32 into memory at address"</para>
        /// </summary>
        public static void Stind_R4() => Emit(OpCodes.Stind_R4);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_R8"/>:</para>
        ///   <para>"Store value of type float64 into memory at address"</para>
        /// </summary>
        public static void Stind_R8() => Emit(OpCodes.Stind_R8);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_Ref"/>:</para>
        ///   <para>"Store value of type object ref (type O) into memory at address"</para>
        /// </summary>
        public static void Stind_Ref() => Emit(OpCodes.Stind_Ref);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_0"/>:</para>
        ///   <para>"Pop a value from stack into local variable 0."</para>
        /// </summary>
        public static void Stloc_0() => Emit(OpCodes.Stloc_0);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_1"/>:</para>
        ///   <para>"Pop a value from stack into local variable 1."</para>
        /// </summary>
        public static void Stloc_1() => Emit(OpCodes.Stloc_1);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_2"/>:</para>
        ///   <para>"Pop a value from stack into local variable 2."</para>
        /// </summary>
        public static void Stloc_2() => Emit(OpCodes.Stloc_2);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_3"/>:</para>
        ///   <para>"Pop a value from stack into local variable 3."</para>
        /// </summary>
        public static void Stloc_3() => Emit(OpCodes.Stloc_3);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sub"/>:</para>
        ///   <para>"Subtract value2 from value1, returning a new value."</para>
        /// </summary>
        public static void Sub() => Emit(OpCodes.Sub);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sub_Ovf"/>:</para>
        ///   <para>"Subtract native int from a native int. Signed result shall fit in same size"</para>
        /// </summary>
        public static void Sub_Ovf() => Emit(OpCodes.Sub_Ovf);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sub_Ovf_Un"/>:</para>
        ///   <para>"Subtract native unsigned int from a native unsigned int. Unsigned result shall fit in same size."</para>
        /// </summary>
        public static void Sub_Ovf_Un() => Emit(OpCodes.Sub_Ovf_Un);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Throw"/>:</para>
        ///   <para>"Throw an exception."</para>
        /// </summary>
        public static void Throw() => Emit(OpCodes.Throw);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Volatile"/>:</para>
        ///   <para>"Subsequent pointer reference is volatile."</para>
        /// </summary>
        public static void Volatile() => Emit(OpCodes.Volatile);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Xor"/>:</para>
        ///   <para>"Bitwise XOR of integer values, returns an integer."</para>
        /// </summary>
        public static void Xor() => Emit(OpCodes.Xor);
        
        #endregion // 
        
        #region string target
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Beq"/>:</para>
        ///   <para>"Branch to target if equal."</para>
        /// </summary>
        public static void Beq(string target) => Emit(OpCodes.Beq, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Beq_S"/>:</para>
        ///   <para>"Branch to target if equal, short form."</para>
        /// </summary>
        public static void Beq_S(string target) => Emit(OpCodes.Beq_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge"/>:</para>
        ///   <para>"Branch to target if greater than or equal to."</para>
        /// </summary>
        public static void Bge(string target) => Emit(OpCodes.Bge, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge_S"/>:</para>
        ///   <para>"Branch to target if greater than or equal to, short form."</para>
        /// </summary>
        public static void Bge_S(string target) => Emit(OpCodes.Bge_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge_Un"/>:</para>
        ///   <para>"Branch to target if greater than or equal to (unsigned or unordered)."</para>
        /// </summary>
        public static void Bge_Un(string target) => Emit(OpCodes.Bge_Un, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge_Un_S"/>:</para>
        ///   <para>"Branch to target if greater than or equal to (unsigned or unordered), short form"</para>
        /// </summary>
        public static void Bge_Un_S(string target) => Emit(OpCodes.Bge_Un_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt"/>:</para>
        ///   <para>"Branch to target if greater than."</para>
        /// </summary>
        public static void Bgt(string target) => Emit(OpCodes.Bgt, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt_S"/>:</para>
        ///   <para>"Branch to target if greater than, short form."</para>
        /// </summary>
        public static void Bgt_S(string target) => Emit(OpCodes.Bgt_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt_Un"/>:</para>
        ///   <para>"Branch to target if greater than (unsigned or unordered)."</para>
        /// </summary>
        public static void Bgt_Un(string target) => Emit(OpCodes.Bgt_Un, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt_Un_S"/>:</para>
        ///   <para>"Branch to target if greater than (unsigned or unordered), short form."</para>
        /// </summary>
        public static void Bgt_Un_S(string target) => Emit(OpCodes.Bgt_Un_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble"/>:</para>
        ///   <para>"Branch to target if less than or equal to."</para>
        /// </summary>
        public static void Ble(string target) => Emit(OpCodes.Ble, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble_S"/>:</para>
        ///   <para>"Branch to target if less than or equal to, short form."</para>
        /// </summary>
        public static void Ble_S(string target) => Emit(OpCodes.Ble_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble_Un"/>:</para>
        ///   <para>"Branch to target if less than or equal to (unsigned or unordered)."</para>
        /// </summary>
        public static void Ble_Un(string target) => Emit(OpCodes.Ble_Un, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble_Un_S"/>:</para>
        ///   <para>"Branch to target if less than or equal to (unsigned or unordered), short form"</para>
        /// </summary>
        public static void Ble_Un_S(string target) => Emit(OpCodes.Ble_Un_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt"/>:</para>
        ///   <para>"Branch to target if less than."</para>
        /// </summary>
        public static void Blt(string target) => Emit(OpCodes.Blt, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt_S"/>:</para>
        ///   <para>"Branch to target if less than, short form."</para>
        /// </summary>
        public static void Blt_S(string target) => Emit(OpCodes.Blt_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt_Un"/>:</para>
        ///   <para>"Branch to target if less than (unsigned or unordered)."</para>
        /// </summary>
        public static void Blt_Un(string target) => Emit(OpCodes.Blt_Un, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt_Un_S"/>:</para>
        ///   <para>"Branch to target if less than (unsigned or unordered), short form."</para>
        /// </summary>
        public static void Blt_Un_S(string target) => Emit(OpCodes.Blt_Un_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bne_Un"/>:</para>
        ///   <para>"Branch to target if unequal or unordered."</para>
        /// </summary>
        public static void Bne_Un(string target) => Emit(OpCodes.Bne_Un, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bne_Un_S"/>:</para>
        ///   <para>"Branch to target if unequal or unordered, short form."</para>
        /// </summary>
        public static void Bne_Un_S(string target) => Emit(OpCodes.Bne_Un_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Br"/>:</para>
        ///   <para>"Branch to target."</para>
        /// </summary>
        public static void Br(string target) => Emit(OpCodes.Br, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Br_S"/>:</para>
        ///   <para>"Branch to target, short form."</para>
        /// </summary>
        public static void Br_S(string target) => Emit(OpCodes.Br_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brfalse"/>:</para>
        ///   <para>"Branch to target if value is zero (false)."</para>
        /// </summary>
        public static void Brfalse(string target) => Emit(OpCodes.Brfalse, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brfalse_S"/>:</para>
        ///   <para>"Branch to target if value is zero (false), short form."</para>
        /// </summary>
        public static void Brfalse_S(string target) => Emit(OpCodes.Brfalse_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brtrue"/>:</para>
        ///   <para>"Branch to target if value is non-zero (true)."</para>
        /// </summary>
        public static void Brtrue(string target) => Emit(OpCodes.Brtrue, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brtrue_S"/>:</para>
        ///   <para>"Branch to target if value is non-zero (true), short form."</para>
        /// </summary>
        public static void Brtrue_S(string target) => Emit(OpCodes.Brtrue_S, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Leave"/>:</para>
        ///   <para>"Exit a protected region of code."</para>
        /// </summary>
        public static void Leave(string target) => Emit(OpCodes.Leave, ( target));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Leave_S"/>:</para>
        ///   <para>"Exit a protected region of code, short form."</para>
        /// </summary>
        public static void Leave_S(string target) => Emit(OpCodes.Leave_S, ( target));
        
        #endregion // string target
        
        #region Type type
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Box"/>:</para>
        ///   <para>"Convert a boxable value to its boxed form"</para>
        /// </summary>
        public static void Box(Type type) => Emit(OpCodes.Box, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Castclass"/>:</para>
        ///   <para>"Cast obj to class."</para>
        /// </summary>
        public static void Castclass(Type type) => Emit(OpCodes.Castclass, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Constrained"/>:</para>
        ///   <para>"Call a virtual method on a type constrained to be type T"</para>
        /// </summary>
        public static void Constrained(Type type) => Emit(OpCodes.Constrained, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cpobj"/>:</para>
        ///   <para>"Copy a value type from src to dest."</para>
        /// </summary>
        public static void Cpobj(Type type) => Emit(OpCodes.Cpobj, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Initobj"/>:</para>
        ///   <para>"Initialize the value at address dest."</para>
        /// </summary>
        public static void Initobj(Type type) => Emit(OpCodes.Initobj, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Isinst"/>:</para>
        ///   <para>"Test if obj is an instance of class, returning null or an instance of that class or interface."</para>
        /// </summary>
        public static void Isinst(Type type) => Emit(OpCodes.Isinst, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldobj"/>:</para>
        ///   <para>"Copy the value stored at address src to the stack."</para>
        /// </summary>
        public static void Ldobj(Type type) => Emit(OpCodes.Ldobj, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldtoken"/>:</para>
        ///   <para>"Convert metadata token to its runtime representation."</para>
        /// </summary>
        public static void Ldtoken(Type type) => Emit(OpCodes.Ldtoken, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mkrefany"/>:</para>
        ///   <para>"Push a typed reference to ptr of type class onto the stack."</para>
        /// </summary>
        public static void Mkrefany(Type type) => Emit(OpCodes.Mkrefany, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Refanyval"/>:</para>
        ///   <para>"Push the address stored in a typed reference."</para>
        /// </summary>
        public static void Refanyval(Type type) => Emit(OpCodes.Refanyval, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sizeof"/>:</para>
        ///   <para>"Push the size, in bytes, of a type as an unsigned int32."</para>
        /// </summary>
        public static void Sizeof(Type type) => Emit(OpCodes.Sizeof, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stobj"/>:</para>
        ///   <para>"Store a value of type typeTok at an address."</para>
        /// </summary>
        public static void Stobj(Type type) => Emit(OpCodes.Stobj, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Unbox"/>:</para>
        ///   <para>"Extract a value-type from obj, its boxed representation."</para>
        /// </summary>
        public static void Unbox(Type type) => Emit(OpCodes.Unbox, ( type));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Unbox_Any"/>:</para>
        ///   <para>"Extract a value-type from obj, its boxed representation"</para>
        /// </summary>
        public static void Unbox_Any(Type type) => Emit(OpCodes.Unbox_Any, ( type));
        
        #endregion // Type type
        
        #region MethodInfo method
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Call"/>:</para>
        ///   <para>"Call method described by method."</para>
        /// </summary>
        public static void Call(MethodInfo method) => Emit(OpCodes.Call, ( method));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Callvirt"/>:</para>
        ///   <para>"Call a method associated with an object."</para>
        /// </summary>
        public static void Callvirt(MethodInfo method) => Emit(OpCodes.Callvirt, ( method));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Jmp"/>:</para>
        ///   <para>"Exit current method and jump to the specified method."</para>
        /// </summary>
        public static void Jmp(MethodInfo method) => Emit(OpCodes.Jmp, ( method));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldftn"/>:</para>
        ///   <para>"Push a pointer to a method referenced by method, on the stack."</para>
        /// </summary>
        public static void Ldftn(MethodInfo method) => Emit(OpCodes.Ldftn, ( method));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldtoken"/>:</para>
        ///   <para>"Convert metadata token to its runtime representation."</para>
        /// </summary>
        public static void Ldtoken(MethodInfo method) => Emit(OpCodes.Ldtoken, ( method));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldvirtftn"/>:</para>
        ///   <para>"Push address of virtual method on the stack."</para>
        /// </summary>
        public static void Ldvirtftn(MethodInfo method) => Emit(OpCodes.Ldvirtftn, ( method));
        
        #endregion // MethodInfo method
        
        #region CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Calli"/>:</para>
        ///   <para>"Call method indicated on the stack with arguments described by callsitedescr."</para>
        /// </summary>
        public static void Calli(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) => Emit(OpCodes.Calli, ( optionalParameterTypes));
        
        #endregion // CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes
        
        #region byte nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_S"/>:</para>
        ///   <para>"Load argument numbered num onto the stack, short form."</para>
        /// </summary>
        public static void Ldarg_S(byte nbr) => Emit(OpCodes.Ldarg_S, ( nbr));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarga_S"/>:</para>
        ///   <para>"Fetch the address of argument argNum, short form."</para>
        /// </summary>
        public static void Ldarga_S(byte nbr) => Emit(OpCodes.Ldarga_S, ( nbr));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_S"/>:</para>
        ///   <para>"Load local variable of index indx onto stack, short form."</para>
        /// </summary>
        public static void Ldloc_S(byte nbr) => Emit(OpCodes.Ldloc_S, ( nbr));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloca_S"/>:</para>
        ///   <para>"Load address of local variable with index indx, short form."</para>
        /// </summary>
        public static void Ldloca_S(byte nbr) => Emit(OpCodes.Ldloca_S, ( nbr));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Starg_S"/>:</para>
        ///   <para>"Store value to the argument numbered num, short form."</para>
        /// </summary>
        public static void Starg_S(byte nbr) => Emit(OpCodes.Starg_S, ( nbr));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_S"/>:</para>
        ///   <para>"Pop a value from stack into local variable indx, short form."</para>
        /// </summary>
        public static void Stloc_S(byte nbr) => Emit(OpCodes.Stloc_S, ( nbr));
        
        #endregion // byte nbr
        
        #region int nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4"/>:</para>
        ///   <para>"Push num of type int32 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4(int nbr) => Emit(OpCodes.Ldc_I4, ( nbr));
        
        #endregion // int nbr
        
        #region sbyte nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_S"/>:</para>
        ///   <para>"Push num onto the stack as int32, short form."</para>
        /// </summary>
        public static void Ldc_I4_S(sbyte nbr) => Emit(OpCodes.Ldc_I4_S, ( nbr));
        
        #endregion // sbyte nbr
        
        #region long nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I8"/>:</para>
        ///   <para>"Push num of type int64 onto the stack as int64."</para>
        /// </summary>
        public static void Ldc_I8(long nbr) => Emit(OpCodes.Ldc_I8, ( nbr));
        
        #endregion // long nbr
        
        #region float nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_R4"/>:</para>
        ///   <para>"Push num of type float32 onto the stack as F."</para>
        /// </summary>
        public static void Ldc_R4(float nbr) => Emit(OpCodes.Ldc_R4, ( nbr));
        
        #endregion // float nbr
        
        #region double nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_R8"/>:</para>
        ///   <para>"Push num of type float64 onto the stack as F."</para>
        /// </summary>
        public static void Ldc_R8(double nbr) => Emit(OpCodes.Ldc_R8, ( nbr));
        
        #endregion // double nbr
        
        #region FieldInfo field
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldfld"/>:</para>
        ///   <para>"Push the value of field of object (or value type) obj, onto the stack."</para>
        /// </summary>
        public static void Ldfld(FieldInfo field) => Emit(OpCodes.Ldfld, ( field));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldflda"/>:</para>
        ///   <para>"Push the address of field of object obj on the stack."</para>
        /// </summary>
        public static void Ldflda(FieldInfo field) => Emit(OpCodes.Ldflda, ( field));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldsfld"/>:</para>
        ///   <para>"Push the value of field on the stack."</para>
        /// </summary>
        public static void Ldsfld(FieldInfo field) => Emit(OpCodes.Ldsfld, ( field));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldsflda"/>:</para>
        ///   <para>"Push the address of the static field, field, on the stack."</para>
        /// </summary>
        public static void Ldsflda(FieldInfo field) => Emit(OpCodes.Ldsflda, ( field));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldtoken"/>:</para>
        ///   <para>"Convert metadata token to its runtime representation."</para>
        /// </summary>
        public static void Ldtoken(FieldInfo field) => Emit(OpCodes.Ldtoken, ( field));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stfld"/>:</para>
        ///   <para>"Replace the value of field of the object obj with value."</para>
        /// </summary>
        public static void Stfld(FieldInfo field) => Emit(OpCodes.Stfld, ( field));
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stsfld"/>:</para>
        ///   <para>"Replace the value of field with val."</para>
        /// </summary>
        public static void Stsfld(FieldInfo field) => Emit(OpCodes.Stsfld, ( field));
        
        #endregion // FieldInfo field
        
        #region string str
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldstr"/>:</para>
        ///   <para>"Push a string object for the literal string."</para>
        /// </summary>
        public static void Ldstr(string str) => Emit(OpCodes.Ldstr, ( str));
        
        #endregion // string str
        
        #region Type itemType
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Newarr"/>:</para>
        ///   <para>"Create a new array with elements of type etype."</para>
        /// </summary>
        public static void Newarr(Type itemType) => Emit(OpCodes.Newarr, ( itemType));
        
        #endregion // Type itemType
        
        #region ConstructorInfo ctor
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Newobj"/>:</para>
        ///   <para>"Allocate an uninitialized object or value type and call ctor."</para>
        /// </summary>
        public static void Newobj(ConstructorInfo ctor) => Emit(OpCodes.Newobj, ( ctor));
        
        #endregion // ConstructorInfo ctor
        
        #region params string[] labelNames
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Switch"/>:</para>
        ///   <para>"Jump to one of n values."</para>
        /// </summary>
        public static void Switch(params string[] labelNames) => Emit(OpCodes.Switch, ( labelNames));
        
        #endregion // params string[] labelNames
        
    }
}
