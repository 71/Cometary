using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Mono.Cecil;

namespace Cometary.Internal
{
    /// <summary>
    /// <see cref="IAssemblyResolver"/> implementation.
    /// </summary>
    internal sealed class AssemblyResolver : IAssemblyResolver
    {
        private readonly Dictionary<string, AssemblyDefinition> assembliesResolved = new Dictionary<string, AssemblyDefinition>();

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(string fullName)
        {
            return Resolve(fullName, new ReaderParameters());
        }

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name, new ReaderParameters());
        }

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            // Fix bad formatting of the fullname:
            // Sometimes the "Version=, Culture=, PublicKeyToken=" part of the assembly
            // appears more than once in the full name; keep only the first part.
            for (int i = 0, commaCount = 0; i < fullName.Length; i++)
            {
                if (fullName[i] != ',')
                    continue;

                if (++commaCount != 4)
                    continue;

                fullName = fullName.Substring(0, i);
                break;
            }

            if (assembliesResolved.TryGetValue(fullName, out AssemblyDefinition assembly))
                return assembly;

            Stream targetStream = GetAssemblyStream(fullName);

            if (targetStream == null)
                return null;

            try
            {
                assembly = AssemblyDefinition.ReadAssembly(targetStream, new ReaderParameters
                {
                    InMemory = true,
                    ReadWrite = false,
                    ReadingMode = ReadingMode.Immediate,
                    AssemblyResolver = this,
                    MetadataResolver = ILVisitor.MetadataResolver
                });
            }
            catch
            {
                // huh that shouldn't happen
                return null;
            }
            finally
            {
                targetStream.Dispose();
            }

            this.assembliesResolved.Add(fullName, assembly);

            return assembly;
        }

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name.FullName, parameters);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to dispose.
        }

        /// <summary>
        /// Attempts to get the stream corresponding to the specified assembly.
        /// </summary>
        private static Stream GetAssemblyStream(string fullname)
        {
            foreach (MetadataReference @ref in Meta.Compilation.References)
            {
                if (@ref is PortableExecutableReference peref &&
                    peref.FilePath != null &&
                    peref.Display == fullname)
                    return File.Open(peref.FilePath, FileMode.Open, FileAccess.Read);
            }

            return null;
        }
    }
}
