using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

AutoWriteIndentation = true;

Context
    .WriteUsings("System", "System.Reflection", "System.Reflection.Emit", "System.Reflection.Metadata")
    .WriteLine()
    .WriteNamespace("Cometary")

    .WriteLine("partial class IL")
    .WriteLine('{')
    .IncreaseIndentation(4);

// Instructions found on https://en.wikipedia.org/wiki/List_of_CIL_instructions
// License: https://creativecommons.org/licenses/by-sa/4.0/
// Script used:
//
//  var table = document.querySelector('.wikitable');
//  var names = table.querySelectorAll('td:nth-child(2)');
//  var dscrs = table.querySelectorAll('td:nth-child(3)');
//  var str = "";
//  for (var i = 0; i < names.length; i++)
//      str += '{ "' + names[i].innerText.trim() + '", "' + dscrs[i].innerText.trim() + '" },\n';
//  return str;
//

Dictionary<string, string> allOpCodes = new Dictionary<string, string>
{
    { "add", "Add two values, returning a new value." },
    { "add.ovf", "Add signed integer values with overflow check." },
    { "add.ovf.un", "Add unsigned integer values with overflow check." },
    { "and", "Bitwise AND of two integral values, returns an integral value." },
    { "arglist", "Return argument list handle for the current method." },
    { "beq <int32 (target)>", "Branch to target if equal." },
    { "beq.s <int8 (target)>", "Branch to target if equal, short form." },
    { "bge <int32 (target)>", "Branch to target if greater than or equal to." },
    { "bge.s <int8 (target)>", "Branch to target if greater than or equal to, short form." },
    { "bge.un <int32 (target)>", "Branch to target if greater than or equal to (unsigned or unordered)." },
    { "bge.un.s <int8 (target)>", "Branch to target if greater than or equal to (unsigned or unordered), short form" },
    { "bgt <int32 (target)>", "Branch to target if greater than." },
    { "bgt.s <int8 (target)>", "Branch to target if greater than, short form." },
    { "bgt.un <int32 (target)>", "Branch to target if greater than (unsigned or unordered)." },
    { "bgt.un.s <int8 (target)>", "Branch to target if greater than (unsigned or unordered), short form." },
    { "ble <int32 (target)>", "Branch to target if less than or equal to." },
    { "ble.s <int8 (target)>", "Branch to target if less than or equal to, short form." },
    { "ble.un <int32 (target)>", "Branch to target if less than or equal to (unsigned or unordered)." },
    { "ble.un.s <int8 (target)>", "Branch to target if less than or equal to (unsigned or unordered), short form" },
    { "blt <int32 (target)>", "Branch to target if less than." },
    { "blt.s <int8 (target)>", "Branch to target if less than, short form." },
    { "blt.un <int32 (target)>", "Branch to target if less than (unsigned or unordered)." },
    { "blt.un.s <int8 (target)>", "Branch to target if less than (unsigned or unordered), short form." },
    { "bne.un <int32 (target)>", "Branch to target if unequal or unordered." },
    { "bne.un.s <int8 (target)>", "Branch to target if unequal or unordered, short form." },
    { "box <typeTok>", "Convert a boxable value to its boxed form" },
    { "br <int32 (target)>", "Branch to target." },
    { "br.s <int8 (target)>", "Branch to target, short form." },
    { "break", "Inform a debugger that a breakpoint has been reached." },
    { "brfalse <int32 (target)>", "Branch to target if value is zero (false)." },
    { "brfalse.s <int8 (target)>", "Branch to target if value is zero (false), short form." },
    { "brinst <int32 (target)>", "Branch to target if value is a non-null object reference (alias for brtrue)." },
    { "brinst.s <int8 (target)>", "Branch to target if value is a non-null object reference, short form (alias for brtrue.s)." },
    { "brnull <int32 (target)>", "Branch to target if value is null (alias for brfalse)." },
    { "brnull.s <int8 (target)>", "Branch to target if value is null (alias for brfalse.s), short form." },
    { "brtrue <int32 (target)>", "Branch to target if value is non-zero (true)." },
    { "brtrue.s <int8 (target)>", "Branch to target if value is non-zero (true), short form." },
    { "brzero <int32 (target)>", "Branch to target if value is zero (alias for brfalse)." },
    { "brzero.s <int8 (target)>", "Branch to target if value is zero (alias for brfalse.s), short form." },
    { "call <method>", "Call method described by method." },
    { "calli <callsitedescr>", "Call method indicated on the stack with arguments described by callsitedescr." },
    { "callvirt <method>", "Call a method associated with an object." },
    { "castclass <class>", "Cast obj to class." },
    { "ceq", "Push 1 (of type int32) if value1 equals value2, else push 0." },
    { "cgt", "Push 1 (of type int32) if value1 > value2, else push 0." },
    { "cgt.un", "Push 1 (of type int32) if value1 > value2, unsigned or unordered, else push 0." },
    { "ckfinite", "Throw ArithmeticException if value is not a finite number." },
    { "clt", "Push 1 (of type int32) if value1 < value2, else push 0." },
    { "clt.un", "Push 1 (of type int32) if value1 < value2, unsigned or unordered, else push 0." },
    { "constrained. <thisType>", "Call a virtual method on a type constrained to be type T" },
    { "conv.i", "Convert to native int, pushing native int on stack." },
    { "conv.i1", "Convert to int8, pushing int32 on stack." },
    { "conv.i2", "Convert to int16, pushing int32 on stack." },
    { "conv.i4", "Convert to int32, pushing int32 on stack." },
    { "conv.i8", "Convert to int64, pushing int64 on stack." },
    { "conv.ovf.i", "Convert to a native int (on the stack as native int) and throw an exception on overflow." },
    { "conv.ovf.i.un", "Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow." },
    { "conv.ovf.i1", "Convert to an int8 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.i1.un", "Convert unsigned to an int8 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.i2", "Convert to an int16 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.i2.un", "Convert unsigned to an int16 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.i4", "Convert to an int32 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.i4.un", "Convert unsigned to an int32 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.i8", "Convert to an int64 (on the stack as int64) and throw an exception on overflow." },
    { "conv.ovf.i8.un", "Convert unsigned to an int64 (on the stack as int64) and throw an exception on overflow." },
    { "conv.ovf.u", "Convert to a native unsigned int (on the stack as native int) and throw an exception on overflow." },
    { "conv.ovf.u.un", "Convert unsigned to a native unsigned int (on the stack as native int) and throw an exception on overflow." },
    { "conv.ovf.u1", "Convert to an unsigned int8 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.u1.un", "Convert unsigned to an unsigned int8 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.u2", "Convert to an unsigned int16 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.u2.un", "Convert unsigned to an unsigned int16 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.u4", "Convert to an unsigned int32 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.u4.un", "Convert unsigned to an unsigned int32 (on the stack as int32) and throw an exception on overflow." },
    { "conv.ovf.u8", "Convert to an unsigned int64 (on the stack as int64) and throw an exception on overflow." },
    { "conv.ovf.u8.un", "Convert unsigned to an unsigned int64 (on the stack as int64) and throw an exception on overflow." },
    { "conv.r.un", "Convert unsigned integer to floating-point, pushing F on stack." },
    { "conv.r4", "Convert to float32, pushing F on stack." },
    { "conv.r8", "Convert to float64, pushing F on stack." },
    { "conv.u", "Convert to native unsigned int, pushing native int on stack." },
    { "conv.u1", "Convert to unsigned int8, pushing int32 on stack." },
    { "conv.u2", "Convert to unsigned int16, pushing int32 on stack." },
    { "conv.u4", "Convert to unsigned int32, pushing int32 on stack." },
    { "conv.u8", "Convert to unsigned int64, pushing int64 on stack." },
    { "cpblk", "Copy data from memory to memory." },
    { "cpobj <typeTok>", "Copy a value type from src to dest." },
    { "div", "Divide two values to return a quotient or floating-point result." },
    { "div.un", "Divide two values, unsigned, returning a quotient." },
    { "dup", "Duplicate the value on the top of the stack." },
    { "endfault", "End fault clause of an exception block." },
    { "endfilter", "End an exception handling filter clause." },
    { "endfinally", "End finally clause of an exception block." },
    { "initblk", "Set all bytes in a block of memory to a given byte value." },
    { "initobj <typeTok>", "Initialize the value at address dest." },
    { "isinst <class>", "Test if obj is an instance of class, returning null or an instance of that class or interface." },
    { "jmp <method>", "Exit current method and jump to the specified method." },
    { "ldarg <uint16 (num)>", "Load argument numbered num onto the stack." },
    { "ldarg.0", "Load argument 0 onto the stack." },
    { "ldarg.1", "Load argument 1 onto the stack." },
    { "ldarg.2", "Load argument 2 onto the stack." },
    { "ldarg.3", "Load argument 3 onto the stack." },
    { "ldarg.s <uint8 (num)>", "Load argument numbered num onto the stack, short form." },
    { "ldarga <uint16 (argNum)>", "Fetch the address of argument argNum." },
    { "ldarga.s <uint8 (argNum)>", "Fetch the address of argument argNum, short form." },
    { "ldc.i4 <int32 (num)>", "Push num of type int32 onto the stack as int32." },
    { "ldc.i4.0", "Push 0 onto the stack as int32." },
    { "ldc.i4.1", "Push 1 onto the stack as int32." },
    { "ldc.i4.2", "Push 2 onto the stack as int32." },
    { "ldc.i4.3", "Push 3 onto the stack as int32." },
    { "ldc.i4.4", "Push 4 onto the stack as int32." },
    { "ldc.i4.5", "Push 5 onto the stack as int32." },
    { "ldc.i4.6", "Push 6 onto the stack as int32." },
    { "ldc.i4.7", "Push 7 onto the stack as int32." },
    { "ldc.i4.8", "Push 8 onto the stack as int32." },
    { "ldc.i4.m1", "Push -1 onto the stack as int32." },
    { "ldc.i4.s <int8 (num)>", "Push num onto the stack as int32, short form." },
    { "ldc.i8 <int64 (num)>", "Push num of type int64 onto the stack as int64." },
    { "ldc.r4 <float32 (num)>", "Push num of type float32 onto the stack as F." },
    { "ldc.r8 <float64 (num)>", "Push num of type float64 onto the stack as F." },
    { "ldelem <typeTok>", "Load the element at index onto the top of the stack." },
    { "ldelem.i", "Load the element with type native int at index onto the top of the stack as a native int." },
    { "ldelem.i1", "Load the element with type int8 at index onto the top of the stack as an int32." },
    { "ldelem.i2", "Load the element with type int16 at index onto the top of the stack as an int32." },
    { "ldelem.i4", "Load the element with type int32 at index onto the top of the stack as an int32." },
    { "ldelem.i8", "Load the element with type int64 at index onto the top of the stack as an int64." },
    { "ldelem.r4", "Load the element with type float32 at index onto the top of the stack as an F" },
    { "ldelem.r8", "Load the element with type float64 at index onto the top of the stack as an F." },
    { "ldelem.ref", "Load the element at index onto the top of the stack as an O. The type of the O is the same as the element type of the array pushed on the CIL stack." },
    { "ldelem.u1", "Load the element with type unsigned int8 at index onto the top of the stack as an int32." },
    { "ldelem.u2", "Load the element with type unsigned int16 at index onto the top of the stack as an int32." },
    { "ldelem.u4", "Load the element with type unsigned int32 at index onto the top of the stack as an int32." },
    { "ldelem.u8", "Load the element with type unsigned int64 at index onto the top of the stack as an int64 (alias for ldelem.i8)." },
    { "ldelema <class>", "Load the address of element at index onto the top of the stack." },
    { "ldfld <field>", "Push the value of field of object (or value type) obj, onto the stack." },
    { "ldflda <field>", "Push the address of field of object obj on the stack." },
    { "ldftn <method>", "Push a pointer to a method referenced by method, on the stack." },
    { "ldind.i", "Indirect load value of type native int as native int on the stack" },
    { "ldind.i1", "Indirect load value of type int8 as int32 on the stack." },
    { "ldind.i2", "Indirect load value of type int16 as int32 on the stack." },
    { "ldind.i4", "Indirect load value of type int32 as int32 on the stack." },
    { "ldind.i8", "Indirect load value of type int64 as int64 on the stack." },
    { "ldind.r4", "Indirect load value of type float32 as F on the stack." },
    { "ldind.r8", "Indirect load value of type float64 as F on the stack." },
    { "ldind.ref", "Indirect load value of type object ref as O on the stack." },
    { "ldind.u1", "Indirect load value of type unsigned int8 as int32 on the stack" },
    { "ldind.u2", "Indirect load value of type unsigned int16 as int32 on the stack" },
    { "ldind.u4", "Indirect load value of type unsigned int32 as int32 on the stack" },
    { "ldind.u8", "Indirect load value of type unsigned int64 as int64 on the stack (alias for ldind.i8)." },
    { "ldlen", "Push the length (of type native unsigned int) of array on the stack." },
    { "ldloc <uint16 (indx)>", "Load local variable of index indx onto stack." },
    { "ldloc.0", "Load local variable 0 onto stack." },
    { "ldloc.1", "Load local variable 1 onto stack." },
    { "ldloc.2", "Load local variable 2 onto stack." },
    { "ldloc.3", "Load local variable 3 onto stack." },
    { "ldloc.s <uint8 (indx)>", "Load local variable of index indx onto stack, short form." },
    { "ldloca <uint16 (indx)>", "Load address of local variable with index indx." },
    { "ldloca.s <uint8 (indx)>", "Load address of local variable with index indx, short form." },
    { "ldnull", "Push a null reference on the stack." },
    { "ldobj <typeTok>", "Copy the value stored at address src to the stack." },
    { "ldsfld <field>", "Push the value of field on the stack." },
    { "ldsflda <field>", "Push the address of the static field, field, on the stack." },
    { "ldstr <string>", "Push a string object for the literal string." },
    { "ldtoken <type>", "Convert metadata token to its runtime representation." },
    { "ldtoken <field>", "Convert metadata token to its runtime representation." },
    { "ldtoken <method>", "Convert metadata token to its runtime representation." },
    { "ldvirtftn <method>", "Push address of virtual method on the stack." },
    { "leave <int32 (target)>", "Exit a protected region of code." },
    { "leave.s <int8 (target)>", "Exit a protected region of code, short form." },
    { "localloc", "Allocate space from the local memory pool." },
    { "mkrefany <class>", "Push a typed reference to ptr of type class onto the stack." },
    { "mul", "Multiply values." },
    { "mul.ovf", "Multiply signed integer values. Signed result shall fit in same size" },
    { "mul.ovf.un", "Multiply unsigned integer values. Unsigned result shall fit in same size" },
    { "neg", "Negate value." },
    { "newarr <etype>", "Create a new array with elements of type etype." },
    { "newobj <ctor>", "Allocate an uninitialized object or value type and call ctor." },
    { "nop", "Do nothing (No operation)." },
    { "not", "Bitwise complement (logical not)." },
    { "or", "Bitwise OR of two integer values, returns an integer." },
    { "pop", "Pop value from the stack." },
    { "readonly.", "Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer" },
    { "refanytype", "Push the type token stored in a typed reference." },
    { "refanyval <type>", "Push the address stored in a typed reference." },
    { "rem", "Remainder when dividing one value by another." },
    { "rem.un", "Remainder when dividing one unsigned value by another." },
    { "ret", "Return from method, possibly with a value." },
    { "rethrow", "Rethrow the current exception." },
    { "shl", "Shift an integer left (shifting in zeros), return an integer." },
    { "shr", "Shift an integer right (shift in sign), return an integer." },
    { "shr.un", "Shift an integer right (shift in zero), return an integer." },
    { "sizeof <typeTok>", "Push the size, in bytes, of a type as an unsigned int32." },
    { "starg <uint16 (num)>", "Store value to the argument numbered num." },
    { "starg.s <uint8 (num)>", "Store value to the argument numbered num, short form." },
    { "stelem <typeTok>", "Replace array element at index with the value on the stack" },
    { "stelem.i", "Replace array element at index with the i value on the stack." },
    { "stelem.i1", "Replace array element at index with the int8 value on the stack." },
    { "stelem.i2", "Replace array element at index with the int16 value on the stack." },
    { "stelem.i4", "Replace array element at index with the int32 value on the stack." },
    { "stelem.i8", "Replace array element at index with the int64 value on the stack." },
    { "stelem.r4", "Replace array element at index with the float32 value on the stack." },
    { "stelem.r8", "Replace array element at index with the float64 value on the stack." },
    { "stelem.ref", "Replace array element at index with the ref value on the stack." },
    { "stfld <field>", "Replace the value of field of the object obj with value." },
    { "stind.i", "Store value of type native int into memory at address" },
    { "stind.i1", "Store value of type int8 into memory at address" },
    { "stind.i2", "Store value of type int16 into memory at address" },
    { "stind.i4", "Store value of type int32 into memory at address" },
    { "stind.i8", "Store value of type int64 into memory at address" },
    { "stind.r4", "Store value of type float32 into memory at address" },
    { "stind.r8", "Store value of type float64 into memory at address" },
    { "stind.ref", "Store value of type object ref (type O) into memory at address" },
    { "stloc <uint16 (indx)>", "Pop a value from stack into local variable indx." },
    { "stloc.0", "Pop a value from stack into local variable 0." },
    { "stloc.1", "Pop a value from stack into local variable 1." },
    { "stloc.2", "Pop a value from stack into local variable 2." },
    { "stloc.3", "Pop a value from stack into local variable 3." },
    { "stloc.s <uint8 (indx)>", "Pop a value from stack into local variable indx, short form." },
    { "stobj <typeTok>", "Store a value of type typeTok at an address." },
    { "stsfld <field>", "Replace the value of field with val." },
    { "sub", "Subtract value2 from value1, returning a new value." },
    { "sub.ovf", "Subtract native int from a native int. Signed result shall fit in same size" },
    { "sub.ovf.un", "Subtract native unsigned int from a native unsigned int. Unsigned result shall fit in same size." },
    { "switch <uint32, int32, int32 (t1..tN)>", "Jump to one of n values." },
    { "tail.", "Subsequent call terminates current method" },
    { "throw", "Throw an exception." },
    { "unaligned. (alignment)", "Subsequent pointer instruction might be unaligned." },
    { "unbox <valuetype>", "Extract a value-type from obj, its boxed representation." },
    { "unbox.any <typeTok>", "Extract a value-type from obj, its boxed representation" },
    { "volatile.", "Subsequent pointer reference is volatile." },
    { "xor", "Bitwise XOR of integer values, returns an integer." }
};

string[] blacklist = {
    "Brinst", "Brnull", "Brzero", "Endfault", "Ldelem_U8", "Ldind_U8", "Unaligned", "Stelem", "Ldelem", "Tail"
};

string Classify(KeyValuePair<string, string> kvp)
{
    string ins = kvp.Key;

    if (ins.Contains("(target)"))
        return "string target";
    else if (ins.Contains("<int32"))
        return "int nbr";
    else if (ins.Contains("<int64"))
        return "long nbr";
    else if (ins.Contains("<float32"))
        return "float nbr";
    else if (ins.Contains("<float64"))
        return "double nbr";
    else if (ins.Contains("<uint8"))
        return "byte nbr";
    else if (ins.Contains("<int8"))
        return "sbyte nbr";
    else if (ins.Contains("<string"))
        return "string str";

    else if (ins.Contains("<field"))
        return "FieldInfo field";
    else if (ins.Contains("<method"))
        return "MethodInfo method";
    else if (ins.Contains("<class") || ins.Contains("<type") || ins.Contains("<valuetype") || ins.Contains("<thisType"))
        return "Type type";
    else if (ins.Contains("<ctor"))
        return "ConstructorInfo ctor";
    else if (ins.Contains("<etype"))
        return "Type itemType";

    else if (ins.Contains("<param"))
        return "int parameterSequence";

    else if (ins.Contains("<callsitedescr"))
        return "CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes";
    else if (ins.Contains("<ins"))
        return "string labelName";
    else if (ins.Contains("switch"))
        return "params string[] labelNames";

    else if (ins.Contains("<"))
        return "SKIP";
    else
        return "";
}

string ConvertName(string name)
{
    return Regex.Replace(
        Regex.Replace(name.Replace('.', '_'), @" <.+>", ""),
        @"(?:_|^)(\w)", match => match.Value.ToUpper()
    ).TrimEnd('_');
}

foreach (var ins in allOpCodes.ToLookup(Classify).Where(x => x.Key != "SKIP"))
{
    string varname = ins.Key == "" ? "" : $", ({ins.Key.Substring(ins.Key.LastIndexOf(' '))})";
    string paramname = ins.Key == "" ? "" : ins.Key;

    Context.WriteLine($"#region {(ins.Key == "" ? "No parameters" : ins.Key)}");

    foreach (var op in ins)
    {
        string opcode = ConvertName(op.Key);
        string lparamname = paramname;
        string lvarname = varname;

        if (blacklist.Any(opcode.StartsWith))
            continue;

        Context.WriteLine("/// <summary>")
               .WriteLine("///   <para>Prints <see cref=\"OpCodes.{0}\"/>:</para>", opcode)
               .WriteLine("///   <para>\"{0}\"</para>", op.Value.Replace("<", "&lt;").Replace(">", "&gt;"))
               .WriteLine("/// </summary>")
               .WriteLine($"public static void {opcode}({lparamname}) => Emit((ILOpCode)OpCodes.{opcode}.Value{lvarname});")
               .WriteLine();
    }

    Context.WriteLine($"#endregion // {ins.Key}")
           .WriteLine();
}

Context
    .DecreaseIndentation(4)
    .WriteLine('}')

    .WriteEnd();