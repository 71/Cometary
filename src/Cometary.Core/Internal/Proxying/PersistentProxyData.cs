using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class PersistentProxyData
    {
        internal readonly Dictionary<int, Func<object, object>> Getters
            = new Dictionary<int, Func<object, object>>();

        internal readonly Dictionary<int, Func<object, object, object>> Setters
            = new Dictionary<int, Func<object, object, object>>();

        internal readonly Dictionary<int, Func<object, object[], object>> Invokers
            = new Dictionary<int, Func<object, object[], object>>();
    }
}
