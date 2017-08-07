using System;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Ryder;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   Represents the context of the <c>CodeGenerator.EmitExpressionCore</c>.
    /// </para>
    /// <para>
    ///   This struct allows the replacement of an inline call by any other call.
    /// </para>
    /// </summary>
    public struct CodeGeneratorContext
    {
        /// <summary>
        ///   Event invoked when a new expression is printed.
        /// </summary>
        public static event PipelineComponent<EmitDelegate> EmitPipeline
        {
            add => Emitters.Add(value ?? throw new ArgumentNullException(nameof(value)));
            remove => Emitters.Remove(value ?? throw new ArgumentNullException(nameof(value)));
        }

        /// <summary>
        ///   Converts a simple <see cref="Action"/> into a <see cref="PipelineComponent{TDelegate}"/> to use
        ///   in <see cref="EmitPipeline"/>.
        /// </summary>
        public static PipelineComponent<EmitDelegate> ToComponent(AlternateEmitDelegate component)
        {
            return next => (context, expression, used) => component(context, expression, used, () => next(context, expression, used));
        }

        #region Hooking
        /// <summary>
        ///   <see cref="MethodRedirection"/> for the <c>CodeGenerator.EmitExpressionCore</c> method.
        /// </summary>
        internal static readonly MethodRedirection EmitRedirection;

        /// <summary>
        /// 
        /// </summary>
        private static readonly Pipeline<EmitDelegate> Emitters;

        static CodeGeneratorContext()
        {
            // Hooking 'CodeGenerator.EmitExpressionCore'
            MethodInfo original = ReflectionHelpers.CodeAnalysisCSharpAssembly
                .GetType("Microsoft.CodeAnalysis.CSharp.CodeGen.CodeGenerator")
                .GetMethod(nameof(EmitExpressionCore), BindingFlags.Instance | BindingFlags.NonPublic);

            MethodInfo replacement = typeof(CodeGeneratorContext)
                .GetMethod(nameof(EmitExpressionCore), BindingFlags.Static | BindingFlags.NonPublic);

            EmitRedirection = Redirection.Redirect(original, replacement, skipChecks: true);
            Emitters = new Pipeline<EmitDelegate>();
        }

        /// <summary>
        ///   Target method of the <see cref="EmitRedirection"/> <see cref="Redirection"/>.
        /// </summary>
        private static void EmitExpressionCore(object codeGenerator, IOperation expression, bool used)
        {
            void Next(CodeGeneratorContext ctx, IOperation op, bool u)
            {
                EmitRedirection.InvokeOriginal(ctx.CodeGenerator, op, u);
            }

            try
            {
                Emitters.MakeDelegate(Next)(new CodeGeneratorContext(codeGenerator, used), expression, used);
            }
            catch (Exception e)
            {
                // We're on our own here, nowhere to log data, nothing to do
                EmitRedirection.InvokeOriginal(codeGenerator, expression, used);
                Console.WriteLine(e);
            }
        }
        #endregion

        /// <summary>
        ///   Static data used to cache reflection objects.
        /// </summary>
        private static readonly PersistentProxyData ProxyData = new PersistentProxyData();

        /// <summary>
        ///   <see cref="Type"/> of the internal <c>CodeGenerator</c> class.
        /// </summary>
        private static readonly Type CodeGeneratorType = typeof(CSharpCompilation)
            .GetTypeInfo().Assembly
            .GetType("Microsoft.CodeAnalysis.CSharp.CodeGen.CodeGenerator");

        /// <summary>
        ///   <see cref="FieldInfo"/> of the <c>ILBuilder</c> field, in the internal <c>CodeGenerator</c> class.
        /// </summary>
        private static readonly FieldInfo ILBuilderField = CodeGeneratorType
            .GetField("_builder", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        ///   <see cref="Type"/> of the internal <c>ILBuilder</c> class.
        /// </summary>
        private static readonly Type ILBuilderType = ILBuilderField.FieldType;

        /// <summary>
        ///   <see cref="MethodInfo"/> of the internal <c>ILBuilder.GetCurrentBuilder()</c> method.
        /// </summary>
        private static readonly MethodInfo BlobBuilderGetter = ILBuilderType.GetMethod("GetCurrentWriter", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        ///   <see cref="FieldInfo"/> of the internal <c>CodeGenerator._diagnostics</c> field.
        /// </summary>
        private static readonly FieldInfo DiagnosticsField = CodeGeneratorType.GetField("_diagnostics", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        ///   <see cref="Proxy"/> to the calling <c>CodeGenerator</c>.
        /// </summary>
        private readonly Proxy CodeGenerator;

        /// <summary>
        ///   <see cref="Proxy"/> to the calling <c>CodeGenerator</c>'s <c>ILBuilder</c>.
        /// </summary>
        private readonly Proxy ILBuilder;

        /// <summary>
        ///   <see cref="Delegate"/> that allows access to the <c>ILBuilder</c>'s
        ///   underlying <see cref="BlobBuilder"/>.
        /// </summary>
        private readonly Func<BlobBuilder> WriterGetter;

        /// <summary>
        ///   The <c>CodeGenerator</c>'s <see cref="DiagnosticBag"/>.
        /// </summary>
        private readonly Action<Diagnostic> AddDiagnostic;

        /// <summary>
        /// 
        /// </summary>
        private readonly bool Used;

        /// <summary>
        ///   Gets the <see cref="BlobBuilder"/> used in this context.
        /// </summary>
        public BlobBuilder Writer => WriterGetter();

        private CodeGeneratorContext(object cg, bool used)
        {
            object ilb = ILBuilderField.GetValue(cg);

            CodeGenerator = new Proxy(cg, CodeGeneratorType, ProxyData);
            ILBuilder = new Proxy(ilb, ILBuilderType, ProxyData);

            WriterGetter = BlobBuilderGetter.CreateDelegate(typeof(Func<BlobBuilder>), ilb) as Func<BlobBuilder>;
            AddDiagnostic = Helpers.MakeAddDiagnostic(DiagnosticsField.GetValue(cg));
            Used = used;
        }

        /// <summary>
        /// 
        /// </summary>
        public void EmitOpCode(ILOpCode opcode) => ILBuilder.Invoke(nameof(EmitOpCode), opcode);

        /// <summary>
        /// 
        /// </summary>
        public void EmitToken(string str) => ILBuilder.Invoke(nameof(EmitToken), str);

        /// <summary>
        /// 
        /// </summary>
        public void EmitSymbolToken(IFieldSymbol symbol, SyntaxNode syntaxNode) => CodeGenerator.Invoke(nameof(EmitSymbolToken), symbol, syntaxNode);

        /// <summary>
        /// 
        /// </summary>
        public void EmitSymbolToken(IMethodSymbol symbol, SyntaxNode syntaxNode, bool encodeAsRawDefinitionToken = false) => CodeGenerator.Invoke(nameof(EmitSymbolToken), symbol, syntaxNode, null, encodeAsRawDefinitionToken);

        /// <summary>
        /// 
        /// </summary>
        public void EmitSymbolToken(ITypeSymbol symbol, SyntaxNode syntaxNode) => CodeGenerator.Invoke(nameof(EmitSymbolToken), symbol, syntaxNode);

        /// <summary>
        /// <para>
        ///   Emits a constant value, and its corresponding opcode.
        /// </para>
        /// <para>
        ///   Supported types: <see langword="null"/>, <see cref="string"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="bool"/>, <see cref="char"/>,
        ///   <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="ushort"/>, <see cref="long"/>, <see cref="ulong"/>,
        ///   <see cref="float"/> and <see cref="double"/>.
        /// </para>
        /// </summary>
        public void EmitConstant(object obj)
        {
            if (obj == null)
            {
                ILBuilder.Invoke("EmitNullConstant");
                return;
            }

            switch (obj)
            {
                case string str:
                    ILBuilder.Invoke("EmitStringConstant", str);
                    break;
                case int int32:
                    ILBuilder.Invoke("EmitIntConstant", int32);
                    break;
                case uint uint32:
                    ILBuilder.Invoke("EmitIntConstant", uint32);
                    break;
                case bool boolean:
                    ILBuilder.Invoke("EmitBoolConstant", boolean);
                    break;
                case char ch:
                    ILBuilder.Invoke("EmitShortConstant", (short)ch);
                    break;
                case byte b:
                    ILBuilder.Invoke("EmitByteConstant", b);
                    break;
                case sbyte sb:
                    ILBuilder.Invoke("EmitSByteConstant", sb);
                    break;
                case short s:
                    ILBuilder.Invoke("EmitShortConstant", s);
                    break;
                case ushort us:
                    ILBuilder.Invoke("EmitUShortConstant", us);
                    break;
                case long int64:
                    ILBuilder.Invoke("EmitLongConstant", int64);
                    break;
                case ulong uint64:
                    ILBuilder.Invoke("EmitLongConstant", uint64);
                    break;
                case float flt:
                    ILBuilder.Invoke("EmitSingleConstant", flt);
                    break;
                case double dbl:
                    ILBuilder.Invoke("EmitDoubleConstant", dbl);
                    break;
            }
        }

        /// <summary>
        /// <para>
        ///   Emits a constant value, omitting its corresponding opcode.
        /// </para>
        /// <para>
        ///   Supported types: <see langword="null"/>, <see cref="string"/>, <see cref="int"/>, <see cref="bool"/>, <see cref="long"/>,
        ///   <see cref="float"/> and <see cref="double"/>.
        /// </para>
        /// </summary>
        public void EmitRawConstant(object obj)
        {
            if (obj == null)
            {
                EmitOpCode(ILOpCode.Ldnull);
                return;
            }

            switch (obj)
            {
                case string str:
                    EmitToken(str);
                    break;
                case sbyte int8:
                    ILBuilder.Invoke("EmitInt8", int8);
                    break;
                case int int32:
                    ILBuilder.Invoke("EmitInt32", int32);
                    break;
                case long int64:
                    ILBuilder.Invoke("EmitInt64", int64);
                    break;
                case float flt:
                    ILBuilder.Invoke("EmitFloat", flt);
                    break;
                case double dbl:
                    ILBuilder.Invoke("EmitDouble", dbl);
                    break;
                case bool boolean:
                    ILBuilder.Invoke("EmitInt32", boolean ? 1 : 0);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EmitOperation(IOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            EmitRedirection.Stop();

            switch (operation.Kind)
            {
                case OperationKind.BlockStatement:
                case OperationKind.VariableDeclarationStatement:
                case OperationKind.SwitchStatement:
                case OperationKind.IfStatement:
                case OperationKind.LoopStatement:
                case OperationKind.LabelStatement:
                case OperationKind.BranchStatement:
                case OperationKind.EmptyStatement:
                case OperationKind.ThrowStatement:
                case OperationKind.ReturnStatement:
                case OperationKind.YieldBreakStatement:
                case OperationKind.LockStatement:
                case OperationKind.TryStatement:
                case OperationKind.UsingStatement:
                case OperationKind.YieldReturnStatement:
                case OperationKind.ExpressionStatement:
                case OperationKind.FixedStatement:
                case OperationKind.LocalFunctionStatement:
                case OperationKind.StopStatement:
                case OperationKind.EndStatement:
                case OperationKind.WithStatement:
                    CodeGenerator.Invoke("EmitStatement", operation);
                    break;
                    
                case OperationKind.LiteralExpression:
                case OperationKind.ConversionExpression:
                case OperationKind.InvocationExpression:
                case OperationKind.ArrayElementReferenceExpression:
                case OperationKind.LocalReferenceExpression:
                case OperationKind.ParameterReferenceExpression:
                case OperationKind.SyntheticLocalReferenceExpression:
                case OperationKind.FieldReferenceExpression:
                case OperationKind.MethodBindingExpression:
                case OperationKind.PropertyReferenceExpression:
                case OperationKind.IndexedPropertyReferenceExpression:
                case OperationKind.EventReferenceExpression:
                case OperationKind.UnaryOperatorExpression:
                case OperationKind.BinaryOperatorExpression:
                case OperationKind.ConditionalChoiceExpression:
                case OperationKind.NullCoalescingExpression:
                case OperationKind.LambdaExpression:
                case OperationKind.ObjectCreationExpression:
                case OperationKind.TypeParameterObjectCreationExpression:
                case OperationKind.ArrayCreationExpression:
                case OperationKind.InstanceReferenceExpression:
                case OperationKind.IsTypeExpression:
                case OperationKind.AwaitExpression:
                case OperationKind.AssignmentExpression:
                case OperationKind.CompoundAssignmentExpression:
                case OperationKind.ParenthesizedExpression:
                case OperationKind.EventAssignmentExpression:
                case OperationKind.ConditionalAccessExpression:
                case OperationKind.ConditionalAccessInstanceExpression:
                case OperationKind.DefaultValueExpression:
                case OperationKind.TypeOfExpression:
                case OperationKind.SizeOfExpression:
                case OperationKind.AddressOfExpression:
                case OperationKind.PointerIndirectionReferenceExpression:
                case OperationKind.UnboundLambdaExpression:
                case OperationKind.IncrementExpression:
                case OperationKind.OmittedArgumentExpression:
                case OperationKind.LateBoundMemberReferenceExpression:
                case OperationKind.PlaceholderExpression:
                    CodeGenerator.Invoke("EmitExpression", operation, /* used */ Used);
                    break;

                case OperationKind.FieldInitializerInCreation:
                case OperationKind.FieldInitializerAtDeclaration:
                case OperationKind.PropertyInitializerInCreation:
                case OperationKind.PropertyInitializerAtDeclaration:
                case OperationKind.ParameterInitializerAtDeclaration:
                case OperationKind.ArrayInitializer:
                case OperationKind.VariableDeclaration:
                case OperationKind.Argument:
                case OperationKind.CatchClause:
                case OperationKind.SwitchCase:
                case OperationKind.SingleValueCaseClause:
                case OperationKind.RelationalCaseClause:
                case OperationKind.RangeCaseClause:
                    EmitRedirection.Start();
                    throw new InvalidOperationException();

                case OperationKind.None:
                case OperationKind.InvalidStatement:
                case OperationKind.InvalidExpression:
                    EmitRedirection.Start();
                    throw new InvalidOperationException();
                    
                default:
                    EmitRedirection.Start();
                    throw new ArgumentOutOfRangeException();
            }

            EmitRedirection.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Report(Diagnostic diagnostic)
        {
            AddDiagnostic(diagnostic);
        }
    }
}
