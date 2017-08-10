using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace Cometary
{
    partial class IL
    {
        #region No parameters
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Add"/>:</para>
        ///   <para>"Add two values, returning a new value."</para>
        /// </summary>
        public static void Add() => Emit((ILOpCode)OpCodes.Add.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Add_Ovf"/>:</para>
        ///   <para>"Add signed integer values with overflow check."</para>
        /// </summary>
        public static void Add_Ovf() => Emit((ILOpCode)OpCodes.Add_Ovf.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Add_Ovf_Un"/>:</para>
        ///   <para>"Add unsigned integer values with overflow check."</para>
        /// </summary>
        public static void Add_Ovf_Un() => Emit((ILOpCode)OpCodes.Add_Ovf_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.And"/>:</para>
        ///   <para>"Bitwise AND of two integral values, returns an integral value."</para>
        /// </summary>
        public static void And() => Emit((ILOpCode)OpCodes.And.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Arglist"/>:</para>
        ///   <para>"Return argument list handle for the current method."</para>
        /// </summary>
        public static void Arglist() => Emit((ILOpCode)OpCodes.Arglist.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Break"/>:</para>
        ///   <para>"Inform a debugger that a breakpoint has been reached."</para>
        /// </summary>
        public static void Break() => Emit((ILOpCode)OpCodes.Break.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ceq"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 equals value2, else push 0."</para>
        /// </summary>
        public static void Ceq() => Emit((ILOpCode)OpCodes.Ceq.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cgt"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &gt; value2, else push 0."</para>
        /// </summary>
        public static void Cgt() => Emit((ILOpCode)OpCodes.Cgt.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cgt_Un"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &gt; value2, unsigned or unordered, else push 0."</para>
        /// </summary>
        public static void Cgt_Un() => Emit((ILOpCode)OpCodes.Cgt_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ckfinite"/>:</para>
        ///   <para>"Throw ArithmeticException if value is not a finite number."</para>
        /// </summary>
        public static void Ckfinite() => Emit((ILOpCode)OpCodes.Ckfinite.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Clt"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &lt; value2, else push 0."</para>
        /// </summary>
        public static void Clt() => Emit((ILOpCode)OpCodes.Clt.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Clt_Un"/>:</para>
        ///   <para>"Push 1 (of type int32) if value1 &lt; value2, unsigned or unordered, else push 0."</para>
        /// </summary>
        public static void Clt_Un() => Emit((ILOpCode)OpCodes.Clt_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I"/>:</para>
        ///   <para>"Convert to native int, pushing native int on stack."</para>
        /// </summary>
        public static void Conv_I() => Emit((ILOpCode)OpCodes.Conv_I.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I1"/>:</para>
        ///   <para>"Convert to int8, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_I1() => Emit((ILOpCode)OpCodes.Conv_I1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I2"/>:</para>
        ///   <para>"Convert to int16, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_I2() => Emit((ILOpCode)OpCodes.Conv_I2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I4"/>:</para>
        ///   <para>"Convert to int32, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_I4() => Emit((ILOpCode)OpCodes.Conv_I4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_I8"/>:</para>
        ///   <para>"Convert to int64, pushing int64 on stack."</para>
        /// </summary>
        public static void Conv_I8() => Emit((ILOpCode)OpCodes.Conv_I8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I"/>:</para>
        ///   <para>"Convert to a native int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I() => Emit((ILOpCode)OpCodes.Conv_Ovf_I.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I_Un"/>:</para>
        ///   <para>"Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_I_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I1"/>:</para>
        ///   <para>"Convert to an int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I1() => Emit((ILOpCode)OpCodes.Conv_Ovf_I1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I1_Un"/>:</para>
        ///   <para>"Convert unsigned to an int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I1_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_I1_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I2"/>:</para>
        ///   <para>"Convert to an int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I2() => Emit((ILOpCode)OpCodes.Conv_Ovf_I2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I2_Un"/>:</para>
        ///   <para>"Convert unsigned to an int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I2_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_I2_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I4"/>:</para>
        ///   <para>"Convert to an int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I4() => Emit((ILOpCode)OpCodes.Conv_Ovf_I4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I4_Un"/>:</para>
        ///   <para>"Convert unsigned to an int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I4_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_I4_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I8"/>:</para>
        ///   <para>"Convert to an int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I8() => Emit((ILOpCode)OpCodes.Conv_Ovf_I8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_I8_Un"/>:</para>
        ///   <para>"Convert unsigned to an int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_I8_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_I8_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U"/>:</para>
        ///   <para>"Convert to a native unsigned int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U() => Emit((ILOpCode)OpCodes.Conv_Ovf_U.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U_Un"/>:</para>
        ///   <para>"Convert unsigned to a native unsigned int (on the stack as native int) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_U_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U1"/>:</para>
        ///   <para>"Convert to an unsigned int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U1() => Emit((ILOpCode)OpCodes.Conv_Ovf_U1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U1_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int8 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U1_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_U1_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U2"/>:</para>
        ///   <para>"Convert to an unsigned int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U2() => Emit((ILOpCode)OpCodes.Conv_Ovf_U2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U2_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int16 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U2_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_U2_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U4"/>:</para>
        ///   <para>"Convert to an unsigned int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U4() => Emit((ILOpCode)OpCodes.Conv_Ovf_U4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U4_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int32 (on the stack as int32) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U4_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_U4_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U8"/>:</para>
        ///   <para>"Convert to an unsigned int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U8() => Emit((ILOpCode)OpCodes.Conv_Ovf_U8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_Ovf_U8_Un"/>:</para>
        ///   <para>"Convert unsigned to an unsigned int64 (on the stack as int64) and throw an exception on overflow."</para>
        /// </summary>
        public static void Conv_Ovf_U8_Un() => Emit((ILOpCode)OpCodes.Conv_Ovf_U8_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_R_Un"/>:</para>
        ///   <para>"Convert unsigned integer to floating-point, pushing F on stack."</para>
        /// </summary>
        public static void Conv_R_Un() => Emit((ILOpCode)OpCodes.Conv_R_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_R4"/>:</para>
        ///   <para>"Convert to float32, pushing F on stack."</para>
        /// </summary>
        public static void Conv_R4() => Emit((ILOpCode)OpCodes.Conv_R4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_R8"/>:</para>
        ///   <para>"Convert to float64, pushing F on stack."</para>
        /// </summary>
        public static void Conv_R8() => Emit((ILOpCode)OpCodes.Conv_R8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U"/>:</para>
        ///   <para>"Convert to native unsigned int, pushing native int on stack."</para>
        /// </summary>
        public static void Conv_U() => Emit((ILOpCode)OpCodes.Conv_U.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U1"/>:</para>
        ///   <para>"Convert to unsigned int8, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_U1() => Emit((ILOpCode)OpCodes.Conv_U1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U2"/>:</para>
        ///   <para>"Convert to unsigned int16, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_U2() => Emit((ILOpCode)OpCodes.Conv_U2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U4"/>:</para>
        ///   <para>"Convert to unsigned int32, pushing int32 on stack."</para>
        /// </summary>
        public static void Conv_U4() => Emit((ILOpCode)OpCodes.Conv_U4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Conv_U8"/>:</para>
        ///   <para>"Convert to unsigned int64, pushing int64 on stack."</para>
        /// </summary>
        public static void Conv_U8() => Emit((ILOpCode)OpCodes.Conv_U8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cpblk"/>:</para>
        ///   <para>"Copy data from memory to memory."</para>
        /// </summary>
        public static void Cpblk() => Emit((ILOpCode)OpCodes.Cpblk.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Div"/>:</para>
        ///   <para>"Divide two values to return a quotient or floating-point result."</para>
        /// </summary>
        public static void Div() => Emit((ILOpCode)OpCodes.Div.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Div_Un"/>:</para>
        ///   <para>"Divide two values, unsigned, returning a quotient."</para>
        /// </summary>
        public static void Div_Un() => Emit((ILOpCode)OpCodes.Div_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Dup"/>:</para>
        ///   <para>"Duplicate the value on the top of the stack."</para>
        /// </summary>
        public static void Dup() => Emit((ILOpCode)OpCodes.Dup.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Endfilter"/>:</para>
        ///   <para>"End an exception handling filter clause."</para>
        /// </summary>
        public static void Endfilter() => Emit((ILOpCode)OpCodes.Endfilter.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Endfinally"/>:</para>
        ///   <para>"End finally clause of an exception block."</para>
        /// </summary>
        public static void Endfinally() => Emit((ILOpCode)OpCodes.Endfinally.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Initblk"/>:</para>
        ///   <para>"Set all bytes in a block of memory to a given byte value."</para>
        /// </summary>
        public static void Initblk() => Emit((ILOpCode)OpCodes.Initblk.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_0"/>:</para>
        ///   <para>"Load argument 0 onto the stack."</para>
        /// </summary>
        public static void Ldarg_0() => Emit((ILOpCode)OpCodes.Ldarg_0.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_1"/>:</para>
        ///   <para>"Load argument 1 onto the stack."</para>
        /// </summary>
        public static void Ldarg_1() => Emit((ILOpCode)OpCodes.Ldarg_1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_2"/>:</para>
        ///   <para>"Load argument 2 onto the stack."</para>
        /// </summary>
        public static void Ldarg_2() => Emit((ILOpCode)OpCodes.Ldarg_2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_3"/>:</para>
        ///   <para>"Load argument 3 onto the stack."</para>
        /// </summary>
        public static void Ldarg_3() => Emit((ILOpCode)OpCodes.Ldarg_3.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_0"/>:</para>
        ///   <para>"Push 0 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_0() => Emit((ILOpCode)OpCodes.Ldc_I4_0.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_1"/>:</para>
        ///   <para>"Push 1 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_1() => Emit((ILOpCode)OpCodes.Ldc_I4_1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_2"/>:</para>
        ///   <para>"Push 2 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_2() => Emit((ILOpCode)OpCodes.Ldc_I4_2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_3"/>:</para>
        ///   <para>"Push 3 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_3() => Emit((ILOpCode)OpCodes.Ldc_I4_3.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_4"/>:</para>
        ///   <para>"Push 4 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_4() => Emit((ILOpCode)OpCodes.Ldc_I4_4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_5"/>:</para>
        ///   <para>"Push 5 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_5() => Emit((ILOpCode)OpCodes.Ldc_I4_5.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_6"/>:</para>
        ///   <para>"Push 6 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_6() => Emit((ILOpCode)OpCodes.Ldc_I4_6.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_7"/>:</para>
        ///   <para>"Push 7 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_7() => Emit((ILOpCode)OpCodes.Ldc_I4_7.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_8"/>:</para>
        ///   <para>"Push 8 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_8() => Emit((ILOpCode)OpCodes.Ldc_I4_8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_M1"/>:</para>
        ///   <para>"Push -1 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4_M1() => Emit((ILOpCode)OpCodes.Ldc_I4_M1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I"/>:</para>
        ///   <para>"Indirect load value of type native int as native int on the stack"</para>
        /// </summary>
        public static void Ldind_I() => Emit((ILOpCode)OpCodes.Ldind_I.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I1"/>:</para>
        ///   <para>"Indirect load value of type int8 as int32 on the stack."</para>
        /// </summary>
        public static void Ldind_I1() => Emit((ILOpCode)OpCodes.Ldind_I1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I2"/>:</para>
        ///   <para>"Indirect load value of type int16 as int32 on the stack."</para>
        /// </summary>
        public static void Ldind_I2() => Emit((ILOpCode)OpCodes.Ldind_I2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I4"/>:</para>
        ///   <para>"Indirect load value of type int32 as int32 on the stack."</para>
        /// </summary>
        public static void Ldind_I4() => Emit((ILOpCode)OpCodes.Ldind_I4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_I8"/>:</para>
        ///   <para>"Indirect load value of type int64 as int64 on the stack."</para>
        /// </summary>
        public static void Ldind_I8() => Emit((ILOpCode)OpCodes.Ldind_I8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_R4"/>:</para>
        ///   <para>"Indirect load value of type float32 as F on the stack."</para>
        /// </summary>
        public static void Ldind_R4() => Emit((ILOpCode)OpCodes.Ldind_R4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_R8"/>:</para>
        ///   <para>"Indirect load value of type float64 as F on the stack."</para>
        /// </summary>
        public static void Ldind_R8() => Emit((ILOpCode)OpCodes.Ldind_R8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_Ref"/>:</para>
        ///   <para>"Indirect load value of type object ref as O on the stack."</para>
        /// </summary>
        public static void Ldind_Ref() => Emit((ILOpCode)OpCodes.Ldind_Ref.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_U1"/>:</para>
        ///   <para>"Indirect load value of type unsigned int8 as int32 on the stack"</para>
        /// </summary>
        public static void Ldind_U1() => Emit((ILOpCode)OpCodes.Ldind_U1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_U2"/>:</para>
        ///   <para>"Indirect load value of type unsigned int16 as int32 on the stack"</para>
        /// </summary>
        public static void Ldind_U2() => Emit((ILOpCode)OpCodes.Ldind_U2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldind_U4"/>:</para>
        ///   <para>"Indirect load value of type unsigned int32 as int32 on the stack"</para>
        /// </summary>
        public static void Ldind_U4() => Emit((ILOpCode)OpCodes.Ldind_U4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldlen"/>:</para>
        ///   <para>"Push the length (of type native unsigned int) of array on the stack."</para>
        /// </summary>
        public static void Ldlen() => Emit((ILOpCode)OpCodes.Ldlen.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_0"/>:</para>
        ///   <para>"Load local variable 0 onto stack."</para>
        /// </summary>
        public static void Ldloc_0() => Emit((ILOpCode)OpCodes.Ldloc_0.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_1"/>:</para>
        ///   <para>"Load local variable 1 onto stack."</para>
        /// </summary>
        public static void Ldloc_1() => Emit((ILOpCode)OpCodes.Ldloc_1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_2"/>:</para>
        ///   <para>"Load local variable 2 onto stack."</para>
        /// </summary>
        public static void Ldloc_2() => Emit((ILOpCode)OpCodes.Ldloc_2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_3"/>:</para>
        ///   <para>"Load local variable 3 onto stack."</para>
        /// </summary>
        public static void Ldloc_3() => Emit((ILOpCode)OpCodes.Ldloc_3.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldnull"/>:</para>
        ///   <para>"Push a null reference on the stack."</para>
        /// </summary>
        public static void Ldnull() => Emit((ILOpCode)OpCodes.Ldnull.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Localloc"/>:</para>
        ///   <para>"Allocate space from the local memory pool."</para>
        /// </summary>
        public static void Localloc() => Emit((ILOpCode)OpCodes.Localloc.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mul"/>:</para>
        ///   <para>"Multiply values."</para>
        /// </summary>
        public static void Mul() => Emit((ILOpCode)OpCodes.Mul.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mul_Ovf"/>:</para>
        ///   <para>"Multiply signed integer values. Signed result shall fit in same size"</para>
        /// </summary>
        public static void Mul_Ovf() => Emit((ILOpCode)OpCodes.Mul_Ovf.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mul_Ovf_Un"/>:</para>
        ///   <para>"Multiply unsigned integer values. Unsigned result shall fit in same size"</para>
        /// </summary>
        public static void Mul_Ovf_Un() => Emit((ILOpCode)OpCodes.Mul_Ovf_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Neg"/>:</para>
        ///   <para>"Negate value."</para>
        /// </summary>
        public static void Neg() => Emit((ILOpCode)OpCodes.Neg.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Nop"/>:</para>
        ///   <para>"Do nothing (No operation)."</para>
        /// </summary>
        public static void Nop() => Emit((ILOpCode)OpCodes.Nop.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Not"/>:</para>
        ///   <para>"Bitwise complement (logical not)."</para>
        /// </summary>
        public static void Not() => Emit((ILOpCode)OpCodes.Not.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Or"/>:</para>
        ///   <para>"Bitwise OR of two integer values, returns an integer."</para>
        /// </summary>
        public static void Or() => Emit((ILOpCode)OpCodes.Or.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Pop"/>:</para>
        ///   <para>"Pop value from the stack."</para>
        /// </summary>
        public static void Pop() => Emit((ILOpCode)OpCodes.Pop.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Readonly"/>:</para>
        ///   <para>"Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer"</para>
        /// </summary>
        public static void Readonly() => Emit((ILOpCode)OpCodes.Readonly.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Refanytype"/>:</para>
        ///   <para>"Push the type token stored in a typed reference."</para>
        /// </summary>
        public static void Refanytype() => Emit((ILOpCode)OpCodes.Refanytype.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Rem"/>:</para>
        ///   <para>"Remainder when dividing one value by another."</para>
        /// </summary>
        public static void Rem() => Emit((ILOpCode)OpCodes.Rem.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Rem_Un"/>:</para>
        ///   <para>"Remainder when dividing one unsigned value by another."</para>
        /// </summary>
        public static void Rem_Un() => Emit((ILOpCode)OpCodes.Rem_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ret"/>:</para>
        ///   <para>"Return from method, possibly with a value."</para>
        /// </summary>
        public static void Ret() => Emit((ILOpCode)OpCodes.Ret.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Rethrow"/>:</para>
        ///   <para>"Rethrow the current exception."</para>
        /// </summary>
        public static void Rethrow() => Emit((ILOpCode)OpCodes.Rethrow.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Shl"/>:</para>
        ///   <para>"Shift an integer left (shifting in zeros), return an integer."</para>
        /// </summary>
        public static void Shl() => Emit((ILOpCode)OpCodes.Shl.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Shr"/>:</para>
        ///   <para>"Shift an integer right (shift in sign), return an integer."</para>
        /// </summary>
        public static void Shr() => Emit((ILOpCode)OpCodes.Shr.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Shr_Un"/>:</para>
        ///   <para>"Shift an integer right (shift in zero), return an integer."</para>
        /// </summary>
        public static void Shr_Un() => Emit((ILOpCode)OpCodes.Shr_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I"/>:</para>
        ///   <para>"Store value of type native int into memory at address"</para>
        /// </summary>
        public static void Stind_I() => Emit((ILOpCode)OpCodes.Stind_I.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I1"/>:</para>
        ///   <para>"Store value of type int8 into memory at address"</para>
        /// </summary>
        public static void Stind_I1() => Emit((ILOpCode)OpCodes.Stind_I1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I2"/>:</para>
        ///   <para>"Store value of type int16 into memory at address"</para>
        /// </summary>
        public static void Stind_I2() => Emit((ILOpCode)OpCodes.Stind_I2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I4"/>:</para>
        ///   <para>"Store value of type int32 into memory at address"</para>
        /// </summary>
        public static void Stind_I4() => Emit((ILOpCode)OpCodes.Stind_I4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_I8"/>:</para>
        ///   <para>"Store value of type int64 into memory at address"</para>
        /// </summary>
        public static void Stind_I8() => Emit((ILOpCode)OpCodes.Stind_I8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_R4"/>:</para>
        ///   <para>"Store value of type float32 into memory at address"</para>
        /// </summary>
        public static void Stind_R4() => Emit((ILOpCode)OpCodes.Stind_R4.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_R8"/>:</para>
        ///   <para>"Store value of type float64 into memory at address"</para>
        /// </summary>
        public static void Stind_R8() => Emit((ILOpCode)OpCodes.Stind_R8.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stind_Ref"/>:</para>
        ///   <para>"Store value of type object ref (type O) into memory at address"</para>
        /// </summary>
        public static void Stind_Ref() => Emit((ILOpCode)OpCodes.Stind_Ref.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_0"/>:</para>
        ///   <para>"Pop a value from stack into local variable 0."</para>
        /// </summary>
        public static void Stloc_0() => Emit((ILOpCode)OpCodes.Stloc_0.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_1"/>:</para>
        ///   <para>"Pop a value from stack into local variable 1."</para>
        /// </summary>
        public static void Stloc_1() => Emit((ILOpCode)OpCodes.Stloc_1.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_2"/>:</para>
        ///   <para>"Pop a value from stack into local variable 2."</para>
        /// </summary>
        public static void Stloc_2() => Emit((ILOpCode)OpCodes.Stloc_2.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_3"/>:</para>
        ///   <para>"Pop a value from stack into local variable 3."</para>
        /// </summary>
        public static void Stloc_3() => Emit((ILOpCode)OpCodes.Stloc_3.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sub"/>:</para>
        ///   <para>"Subtract value2 from value1, returning a new value."</para>
        /// </summary>
        public static void Sub() => Emit((ILOpCode)OpCodes.Sub.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sub_Ovf"/>:</para>
        ///   <para>"Subtract native int from a native int. Signed result shall fit in same size"</para>
        /// </summary>
        public static void Sub_Ovf() => Emit((ILOpCode)OpCodes.Sub_Ovf.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sub_Ovf_Un"/>:</para>
        ///   <para>"Subtract native unsigned int from a native unsigned int. Unsigned result shall fit in same size."</para>
        /// </summary>
        public static void Sub_Ovf_Un() => Emit((ILOpCode)OpCodes.Sub_Ovf_Un.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Throw"/>:</para>
        ///   <para>"Throw an exception."</para>
        /// </summary>
        public static void Throw() => Emit((ILOpCode)OpCodes.Throw.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Volatile"/>:</para>
        ///   <para>"Subsequent pointer reference is volatile."</para>
        /// </summary>
        public static void Volatile() => Emit((ILOpCode)OpCodes.Volatile.Value);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Xor"/>:</para>
        ///   <para>"Bitwise XOR of integer values, returns an integer."</para>
        /// </summary>
        public static void Xor() => Emit((ILOpCode)OpCodes.Xor.Value);
        
        #endregion // 
        
        #region string target
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Beq"/>:</para>
        ///   <para>"Branch to target if equal."</para>
        /// </summary>
        public static void Beq(string target) => Emit((ILOpCode)OpCodes.Beq.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Beq_S"/>:</para>
        ///   <para>"Branch to target if equal, short form."</para>
        /// </summary>
        public static void Beq_S(string target) => Emit((ILOpCode)OpCodes.Beq_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge"/>:</para>
        ///   <para>"Branch to target if greater than or equal to."</para>
        /// </summary>
        public static void Bge(string target) => Emit((ILOpCode)OpCodes.Bge.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge_S"/>:</para>
        ///   <para>"Branch to target if greater than or equal to, short form."</para>
        /// </summary>
        public static void Bge_S(string target) => Emit((ILOpCode)OpCodes.Bge_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge_Un"/>:</para>
        ///   <para>"Branch to target if greater than or equal to (unsigned or unordered)."</para>
        /// </summary>
        public static void Bge_Un(string target) => Emit((ILOpCode)OpCodes.Bge_Un.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bge_Un_S"/>:</para>
        ///   <para>"Branch to target if greater than or equal to (unsigned or unordered), short form"</para>
        /// </summary>
        public static void Bge_Un_S(string target) => Emit((ILOpCode)OpCodes.Bge_Un_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt"/>:</para>
        ///   <para>"Branch to target if greater than."</para>
        /// </summary>
        public static void Bgt(string target) => Emit((ILOpCode)OpCodes.Bgt.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt_S"/>:</para>
        ///   <para>"Branch to target if greater than, short form."</para>
        /// </summary>
        public static void Bgt_S(string target) => Emit((ILOpCode)OpCodes.Bgt_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt_Un"/>:</para>
        ///   <para>"Branch to target if greater than (unsigned or unordered)."</para>
        /// </summary>
        public static void Bgt_Un(string target) => Emit((ILOpCode)OpCodes.Bgt_Un.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bgt_Un_S"/>:</para>
        ///   <para>"Branch to target if greater than (unsigned or unordered), short form."</para>
        /// </summary>
        public static void Bgt_Un_S(string target) => Emit((ILOpCode)OpCodes.Bgt_Un_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble"/>:</para>
        ///   <para>"Branch to target if less than or equal to."</para>
        /// </summary>
        public static void Ble(string target) => Emit((ILOpCode)OpCodes.Ble.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble_S"/>:</para>
        ///   <para>"Branch to target if less than or equal to, short form."</para>
        /// </summary>
        public static void Ble_S(string target) => Emit((ILOpCode)OpCodes.Ble_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble_Un"/>:</para>
        ///   <para>"Branch to target if less than or equal to (unsigned or unordered)."</para>
        /// </summary>
        public static void Ble_Un(string target) => Emit((ILOpCode)OpCodes.Ble_Un.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ble_Un_S"/>:</para>
        ///   <para>"Branch to target if less than or equal to (unsigned or unordered), short form"</para>
        /// </summary>
        public static void Ble_Un_S(string target) => Emit((ILOpCode)OpCodes.Ble_Un_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt"/>:</para>
        ///   <para>"Branch to target if less than."</para>
        /// </summary>
        public static void Blt(string target) => Emit((ILOpCode)OpCodes.Blt.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt_S"/>:</para>
        ///   <para>"Branch to target if less than, short form."</para>
        /// </summary>
        public static void Blt_S(string target) => Emit((ILOpCode)OpCodes.Blt_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt_Un"/>:</para>
        ///   <para>"Branch to target if less than (unsigned or unordered)."</para>
        /// </summary>
        public static void Blt_Un(string target) => Emit((ILOpCode)OpCodes.Blt_Un.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Blt_Un_S"/>:</para>
        ///   <para>"Branch to target if less than (unsigned or unordered), short form."</para>
        /// </summary>
        public static void Blt_Un_S(string target) => Emit((ILOpCode)OpCodes.Blt_Un_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bne_Un"/>:</para>
        ///   <para>"Branch to target if unequal or unordered."</para>
        /// </summary>
        public static void Bne_Un(string target) => Emit((ILOpCode)OpCodes.Bne_Un.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Bne_Un_S"/>:</para>
        ///   <para>"Branch to target if unequal or unordered, short form."</para>
        /// </summary>
        public static void Bne_Un_S(string target) => Emit((ILOpCode)OpCodes.Bne_Un_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Br"/>:</para>
        ///   <para>"Branch to target."</para>
        /// </summary>
        public static void Br(string target) => Emit((ILOpCode)OpCodes.Br.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Br_S"/>:</para>
        ///   <para>"Branch to target, short form."</para>
        /// </summary>
        public static void Br_S(string target) => Emit((ILOpCode)OpCodes.Br_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brfalse"/>:</para>
        ///   <para>"Branch to target if value is zero (false)."</para>
        /// </summary>
        public static void Brfalse(string target) => Emit((ILOpCode)OpCodes.Brfalse.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brfalse_S"/>:</para>
        ///   <para>"Branch to target if value is zero (false), short form."</para>
        /// </summary>
        public static void Brfalse_S(string target) => Emit((ILOpCode)OpCodes.Brfalse_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brtrue"/>:</para>
        ///   <para>"Branch to target if value is non-zero (true)."</para>
        /// </summary>
        public static void Brtrue(string target) => Emit((ILOpCode)OpCodes.Brtrue.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Brtrue_S"/>:</para>
        ///   <para>"Branch to target if value is non-zero (true), short form."</para>
        /// </summary>
        public static void Brtrue_S(string target) => Emit((ILOpCode)OpCodes.Brtrue_S.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Leave"/>:</para>
        ///   <para>"Exit a protected region of code."</para>
        /// </summary>
        public static void Leave(string target) => Emit((ILOpCode)OpCodes.Leave.Value, target);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Leave_S"/>:</para>
        ///   <para>"Exit a protected region of code, short form."</para>
        /// </summary>
        public static void Leave_S(string target) => Emit((ILOpCode)OpCodes.Leave_S.Value, target);
        
        #endregion // string target
        
        #region Type type
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Box"/>:</para>
        ///   <para>"Convert a boxable value to its boxed form"</para>
        /// </summary>
        public static void Box(Type type) => Emit((ILOpCode)OpCodes.Box.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Castclass"/>:</para>
        ///   <para>"Cast obj to class."</para>
        /// </summary>
        public static void Castclass(Type type) => Emit((ILOpCode)OpCodes.Castclass.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Constrained"/>:</para>
        ///   <para>"Call a virtual method on a type constrained to be type T"</para>
        /// </summary>
        public static void Constrained(Type type) => Emit((ILOpCode)OpCodes.Constrained.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Cpobj"/>:</para>
        ///   <para>"Copy a value type from src to dest."</para>
        /// </summary>
        public static void Cpobj(Type type) => Emit((ILOpCode)OpCodes.Cpobj.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Initobj"/>:</para>
        ///   <para>"Initialize the value at address dest."</para>
        /// </summary>
        public static void Initobj(Type type) => Emit((ILOpCode)OpCodes.Initobj.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Isinst"/>:</para>
        ///   <para>"Test if obj is an instance of class, returning null or an instance of that class or interface."</para>
        /// </summary>
        public static void Isinst(Type type) => Emit((ILOpCode)OpCodes.Isinst.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldobj"/>:</para>
        ///   <para>"Copy the value stored at address src to the stack."</para>
        /// </summary>
        public static void Ldobj(Type type) => Emit((ILOpCode)OpCodes.Ldobj.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Mkrefany"/>:</para>
        ///   <para>"Push a typed reference to ptr of type class onto the stack."</para>
        /// </summary>
        public static void Mkrefany(Type type) => Emit((ILOpCode)OpCodes.Mkrefany.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Refanyval"/>:</para>
        ///   <para>"Push the address stored in a typed reference."</para>
        /// </summary>
        public static void Refanyval(Type type) => Emit((ILOpCode)OpCodes.Refanyval.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Sizeof"/>:</para>
        ///   <para>"Push the size, in bytes, of a type as an unsigned int32."</para>
        /// </summary>
        public static void Sizeof(Type type) => Emit((ILOpCode)OpCodes.Sizeof.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stobj"/>:</para>
        ///   <para>"Store a value of type typeTok at an address."</para>
        /// </summary>
        public static void Stobj(Type type) => Emit((ILOpCode)OpCodes.Stobj.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Unbox"/>:</para>
        ///   <para>"Extract a value-type from obj, its boxed representation."</para>
        /// </summary>
        public static void Unbox(Type type) => Emit((ILOpCode)OpCodes.Unbox.Value, type);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Unbox_Any"/>:</para>
        ///   <para>"Extract a value-type from obj, its boxed representation"</para>
        /// </summary>
        public static void Unbox_Any(Type type) => Emit((ILOpCode)OpCodes.Unbox_Any.Value, type);
        
        #endregion // Type type
        
        #region object methodCall
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Call"/>:</para>
        ///   <para>"Call method described by method."</para>
        /// </summary>
        public static void Call(object methodCall) => Emit((ILOpCode)OpCodes.Call.Value, methodCall);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Callvirt"/>:</para>
        ///   <para>"Call a method associated with an object."</para>
        /// </summary>
        public static void Callvirt(object methodCall) => Emit((ILOpCode)OpCodes.Callvirt.Value, methodCall);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Jmp"/>:</para>
        ///   <para>"Exit current method and jump to the specified method."</para>
        /// </summary>
        public static void Jmp(object methodCall) => Emit((ILOpCode)OpCodes.Jmp.Value, methodCall);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldftn"/>:</para>
        ///   <para>"Push a pointer to a method referenced by method, on the stack."</para>
        /// </summary>
        public static void Ldftn(object methodCall) => Emit((ILOpCode)OpCodes.Ldftn.Value, methodCall);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldvirtftn"/>:</para>
        ///   <para>"Push address of virtual method on the stack."</para>
        /// </summary>
        public static void Ldvirtftn(object methodCall) => Emit((ILOpCode)OpCodes.Ldvirtftn.Value, methodCall);
        
        #endregion // object methodCall
        
        #region byte nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarg_S"/>:</para>
        ///   <para>"Load argument numbered num onto the stack, short form."</para>
        /// </summary>
        public static void Ldarg_S(byte nbr) => Emit((ILOpCode)OpCodes.Ldarg_S.Value, nbr);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldarga_S"/>:</para>
        ///   <para>"Fetch the address of argument argNum, short form."</para>
        /// </summary>
        public static void Ldarga_S(byte nbr) => Emit((ILOpCode)OpCodes.Ldarga_S.Value, nbr);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloc_S"/>:</para>
        ///   <para>"Load local variable of index indx onto stack, short form."</para>
        /// </summary>
        public static void Ldloc_S(byte nbr) => Emit((ILOpCode)OpCodes.Ldloc_S.Value, nbr);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldloca_S"/>:</para>
        ///   <para>"Load address of local variable with index indx, short form."</para>
        /// </summary>
        public static void Ldloca_S(byte nbr) => Emit((ILOpCode)OpCodes.Ldloca_S.Value, nbr);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Starg_S"/>:</para>
        ///   <para>"Store value to the argument numbered num, short form."</para>
        /// </summary>
        public static void Starg_S(byte nbr) => Emit((ILOpCode)OpCodes.Starg_S.Value, nbr);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stloc_S"/>:</para>
        ///   <para>"Pop a value from stack into local variable indx, short form."</para>
        /// </summary>
        public static void Stloc_S(byte nbr) => Emit((ILOpCode)OpCodes.Stloc_S.Value, nbr);
        
        #endregion // byte nbr
        
        #region int nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4"/>:</para>
        ///   <para>"Push num of type int32 onto the stack as int32."</para>
        /// </summary>
        public static void Ldc_I4(int nbr) => Emit((ILOpCode)OpCodes.Ldc_I4.Value, nbr);
        
        #endregion // int nbr
        
        #region sbyte nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I4_S"/>:</para>
        ///   <para>"Push num onto the stack as int32, short form."</para>
        /// </summary>
        public static void Ldc_I4_S(sbyte nbr) => Emit((ILOpCode)OpCodes.Ldc_I4_S.Value, nbr);
        
        #endregion // sbyte nbr
        
        #region long nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_I8"/>:</para>
        ///   <para>"Push num of type int64 onto the stack as int64."</para>
        /// </summary>
        public static void Ldc_I8(long nbr) => Emit((ILOpCode)OpCodes.Ldc_I8.Value, nbr);
        
        #endregion // long nbr
        
        #region float nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_R4"/>:</para>
        ///   <para>"Push num of type float32 onto the stack as F."</para>
        /// </summary>
        public static void Ldc_R4(float nbr) => Emit((ILOpCode)OpCodes.Ldc_R4.Value, nbr);
        
        #endregion // float nbr
        
        #region double nbr
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldc_R8"/>:</para>
        ///   <para>"Push num of type float64 onto the stack as F."</para>
        /// </summary>
        public static void Ldc_R8(double nbr) => Emit((ILOpCode)OpCodes.Ldc_R8.Value, nbr);
        
        #endregion // double nbr
        
        #region object fieldAccess
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldfld"/>:</para>
        ///   <para>"Push the value of field of object (or value type) obj, onto the stack."</para>
        /// </summary>
        public static void Ldfld(object fieldAccess) => Emit((ILOpCode)OpCodes.Ldfld.Value, fieldAccess);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldflda"/>:</para>
        ///   <para>"Push the address of field of object obj on the stack."</para>
        /// </summary>
        public static void Ldflda(object fieldAccess) => Emit((ILOpCode)OpCodes.Ldflda.Value, fieldAccess);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldsfld"/>:</para>
        ///   <para>"Push the value of field on the stack."</para>
        /// </summary>
        public static void Ldsfld(object fieldAccess) => Emit((ILOpCode)OpCodes.Ldsfld.Value, fieldAccess);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldsflda"/>:</para>
        ///   <para>"Push the address of the static field, field, on the stack."</para>
        /// </summary>
        public static void Ldsflda(object fieldAccess) => Emit((ILOpCode)OpCodes.Ldsflda.Value, fieldAccess);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stfld"/>:</para>
        ///   <para>"Replace the value of field of the object obj with value."</para>
        /// </summary>
        public static void Stfld(object fieldAccess) => Emit((ILOpCode)OpCodes.Stfld.Value, fieldAccess);
        
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Stsfld"/>:</para>
        ///   <para>"Replace the value of field with val."</para>
        /// </summary>
        public static void Stsfld(object fieldAccess) => Emit((ILOpCode)OpCodes.Stsfld.Value, fieldAccess);
        
        #endregion // object fieldAccess
        
        #region string str
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Ldstr"/>:</para>
        ///   <para>"Push a string object for the literal string."</para>
        /// </summary>
        public static void Ldstr(string str) => Emit((ILOpCode)OpCodes.Ldstr.Value, str);
        
        #endregion // string str
        
        #region Type itemType
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Newarr"/>:</para>
        ///   <para>"Create a new array with elements of type etype."</para>
        /// </summary>
        public static void Newarr(Type itemType) => Emit((ILOpCode)OpCodes.Newarr.Value, itemType);
        
        #endregion // Type itemType
        
        #region object ctorCall
        /// <summary>
        ///   <para>Prints <see cref="OpCodes.Newobj"/>:</para>
        ///   <para>"Allocate an uninitialized object or value type and call ctor."</para>
        /// </summary>
        public static void Newobj(object ctorCall) => Emit((ILOpCode)OpCodes.Newobj.Value, ctorCall);
        
        #endregion // object ctorCall
        
    }
}
