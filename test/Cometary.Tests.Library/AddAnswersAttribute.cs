using System;
using System.Collections.Generic;

namespace Cometary.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AddAnswersAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            yield return new AddAnswersEditor();
        }
    }
}
