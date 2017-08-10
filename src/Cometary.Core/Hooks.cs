using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Ryder;

namespace Cometary
{
    /*
     * This class contains the static Hooks placed on Diagnostic.IsSuppressed,
     * Compilation.CheckOptionsAndCreateModuleBuilder, etc.
     * 
     * Here, everything is static. Right now the code is a mess, but it'd be nice:
     *  - Hooks are static, and completely independant. They don't care about CometaryManager,
     *    or anything of the sort.
     *  - CometaryManager is never static, and always used for a single compilation. It should be
     *    completely dependant on a compilation, and never share state.
     */

    /// <summary>
    ///   Provides access to global methods used as replacement
    ///   by <see cref="Redirection"/>s.
    /// </summary>
    internal static class Hooks
    {
        #region Diagnostics
		/// <summary>
        ///   List of <see cref="Diagnostic"/> predicates that may allow
        ///   suppression of certain errors.
        /// </summary>
        internal static readonly List<Predicate<Diagnostic>> DiagnosticPredicates;

        /// <summary>
        ///   <see cref="MethodRedirection"/> that handles
        ///   redirection of the <see cref="Diagnostic.IsSuppressed"/> property.
        /// </summary>
        internal static readonly MethodRedirection DiagnosticSuppressionRedirection;

        /// <summary>
        ///   Returns whether or not the specified <see cref="Diagnostic"/> is suppressed,
        ///   either by design (via <see cref="Diagnostic.IsSuppressed"/>), or by redirection
        ///   (via <see cref="CompilationEditor.SuppressDiagnostic"/>).
        /// </summary>
        internal static bool IsSuppressed(Diagnostic diagnostic)
        {
            if (diagnostic == null)
                return false;

            DiagnosticSuppressionRedirection.Stop();

            if (diagnostic.IsSuppressed)
            {
                DiagnosticSuppressionRedirection.Start();
                return true;
            }

            var predicates = DiagnosticPredicates;

            try
            {
                for (int i = 0; i < predicates.Count; i++)
                {
                    if (predicates[i](diagnostic))
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                DiagnosticSuppressionRedirection.Start();
            }
        }
        #endregion

        #region Compilation
        /// <summary>
        ///   <see cref="MethodRedirection"/> that handles
        ///   redirection of the <see cref="CheckOptionsAndCreateModuleBuilder"/> method.
        /// </summary>
        internal static readonly ObservableRedirection CompilationRedirection;

        /// <summary>
        ///   List of <see cref="CSharpCompilation"/>s that have already been modified.
        ///   This list is kept to ensure no compilation is modified more than once.
        /// </summary>
        internal static readonly List<CSharpCompilation> ModifiedCompilations;

        /// <summary>
        ///   Injects the custom <see cref="CSharpCompilation"/> to the emitting process,
        ///   and resumes it.
        /// </summary>
        /// <seealso href="http://source.roslyn.io/#Microsoft.CodeAnalysis/Compilation/Compilation.cs,42341c66e909e676"/>
        private static void CheckOptionsAndCreateModuleBuilder(RedirectionContext context)
        {
            // Sender is a CSharpCompilation
            CSharpCompilation compilation = (CSharpCompilation)context.Sender;
            CSharpCompilation clone = compilation.Clone();

            // First argument is a DiagnosticBag
            Action<Diagnostic> addDiagnostic = Helpers.MakeAddDiagnostic(context.Arguments[0]);

            object GetOriginal(CSharpCompilation newCompilation)
            {
                object[] args = new object[context.Arguments.Count];
                context.Arguments.CopyTo(args, 0);

                newCompilation.CopyTo(compilation);

                return context.Invoke(args);
            }

            // CancellationToken should be last argument, but whatever.
            CancellationToken cancellationToken = context.Arguments.OfType<CancellationToken>().FirstOrDefault();

            // Edit the compilation (if a matching CometaryManager is found)
            CompilationRedirection.Stop();

            using (CompilationProcessor manager = CompilationProcessor.Create(GetOriginal, addDiagnostic))
            {
                manager.RegisterAttributes(compilation.Assembly);

                // Edit the compilation, and emit it.
                if (manager.TryEditCompilation(compilation, cancellationToken, out CSharpCompilation _, out object moduleBuilder))
                {
                    // No error, we can keep going
                    context.ReturnValue = moduleBuilder;

                    addDiagnostic(Diagnostic.Create(
                        id: "ProcessSuccess",
                        category: Common.DiagnosticsCategory,
                        message: "Successfully edited the emitted compilation.",
                        severity: DiagnosticSeverity.Info,
                        defaultSeverity: DiagnosticSeverity.Info,
                        isEnabledByDefault: true,
                        warningLevel: -1,
                        isSuppressed: false));
                }
                else
                {
                    // Keep going as if we were never here (the errors will be reported anyways)
                    clone.CopyTo(compilation);

                    context.ReturnValue = context.Invoke(context.Arguments.ToArray());
                }
            }

            CompilationRedirection.Start();
        }
        #endregion

        /// <summary>
        ///   Ensures all hooks have been set up.
        /// </summary>
        internal static void EnsureInitialized()
        {
            // Calling this method for the first time calls .cctor(), so there's nothing to do here.
        }

        /// <summary>
        ///   Ensures all hooks are active.
        /// </summary>
        internal static void EnsureActive()
        {
            CompilationRedirection.Start();
        }

        static Hooks()
        {
            #region Diagnostics
            DiagnosticPredicates = new List<Predicate<Diagnostic>>();

            // Initialize 'Diagnostic.IsSuppressed' redirection.
            MethodInfo getIsSuppressed = typeof(Diagnostic)
                .GetTypeInfo().Assembly
                .GetType("Microsoft.CodeAnalysis.DiagnosticWithInfo")
                .GetProperty(nameof(Diagnostic.IsSuppressed), BindingFlags.Instance | BindingFlags.Public)
                .GetGetMethod();

            MethodInfo getCustomIsSuppressed = typeof(Hooks)
                .GetMethod(nameof(IsSuppressed), BindingFlags.Static | BindingFlags.NonPublic);

            IsSuppressed(null);

            // Make sure the method has been jitted
            try
            {
                Diagnostic diagnostic = getIsSuppressed.DeclaringType
                    .GetTypeInfo().DeclaredConstructors.First()
                    .Invoke(new object[] { null, null, true }) as Diagnostic;

                getIsSuppressed.Invoke(diagnostic, null);
            }
            catch (TargetInvocationException)
            {
                // Happened on purpose!
            }

            DiagnosticSuppressionRedirection = Redirection.Redirect(getIsSuppressed, getCustomIsSuppressed);
            #endregion

            #region Compilation
            ModifiedCompilations = new List<CSharpCompilation>();

            MethodInfo originalMethod = typeof(Compilation)
                .GetMethod(nameof(CheckOptionsAndCreateModuleBuilder), BindingFlags.Instance | BindingFlags.NonPublic);

            try
            {
                CSharpCompilation csc = CSharpCompilation.Create("Init");
                ParameterInfo[] parameters = originalMethod.GetParameters();
                object[] arguments = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    Type paramType = parameters[i].ParameterType;

                    arguments[i] = paramType.GetTypeInfo().IsValueType
                        ? Activator.CreateInstance(paramType, true) // struct
                        : null; // class
                }

                originalMethod.Invoke(csc, arguments);
            }
            catch (TargetInvocationException)
            {
                // Happened on purpose!
            }

            CompilationRedirection = Redirection.Observe(originalMethod);
            CompilationRedirection.Subscribe(CheckOptionsAndCreateModuleBuilder);
            #endregion
        }
    }
}
