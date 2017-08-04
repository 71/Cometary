using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        private static readonly Lazy<Func<Delegate, object, Delegate>> WithTargetCore;

        static Extensions()
        {
            WithTargetCore = Helpers.MakeLazyDelegate<Func<Delegate, object, Delegate>>(nameof(WithTargetCore), il =>
            {
                // Clone delegate
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic));

                // Change the cloned delegate's target field
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, typeof(Delegate).GetField("_target", BindingFlags.Instance | BindingFlags.NonPublic));

                // Return the cloned delegate
                il.Emit(OpCodes.Ret);
            });
        }

        /// <summary>
        ///   Returns a clone of the specified <paramref name="delegate"/>,
        ///   with its target changed.
        /// </summary>
        public static Delegate WithTarget(this Delegate @delegate, object target)
        {
            if (@delegate == null)
                throw new ArgumentNullException(nameof(@delegate));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return WithTargetCore.Value(@delegate, target);
        }
    }
}
