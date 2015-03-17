using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TsdLib.CodeGenerator
{
    /// <summary>
    /// Contains functionality to dynamically generate .NET source code and/or assemblies.
    /// </summary>
    public class DynamicCompiler
    {
        private readonly Language _language;
        private readonly string _tempPath;
        private readonly string _assemblyDirectory;

        /// <summary>
        /// Initialize a new code generator.
        /// </summary>
        /// <param name="language">Generate C# or Visual Basic code.</param>
        /// <param name="assemblyDirectory">The absolute path to the directory where referenced assemblies can be located.</param>
        public DynamicCompiler(Language language, string assemblyDirectory)
        {
            _language = language;
            _tempPath = Path.Combine(Path.GetTempPath(), "TsdLib");
            if (!Directory.Exists(_tempPath))
                Directory.CreateDirectory(_tempPath);
            _assemblyDirectory = assemblyDirectory;
        }

        /// <summary>
        /// Generates an assembly from the specified XML instrument definition file(s) and test sequence source code file.
        /// </summary>
        /// <param name="testSequenceSourceCode">String containing the test sequence source code.</param>
        /// <param name="referencedAssemblies">Names of assemblies to be referenced by the test sequence assembly.</param>
        /// <returns>Absolute path the the generated assembly.</returns>
        public string GenerateTestSequenceAssembly(string testSequenceSourceCode, IEnumerable<string> referencedAssemblies)
        {

            CodeDomProvider provider = CodeDomProvider.CreateProvider(_language.ToString());
            
            string dllPath = Path.Combine(_tempPath, "sequence.dll");

            CompilerParameters cp = new CompilerParameters
            {
                IncludeDebugInformation = true,
                OutputAssembly = dllPath
            };

#if DEBUG
            cp.CompilerOptions += " /d:DEBUG";
#endif
#if TRACE
            cp.CompilerOptions += " /d:TRACE";
#endif
            
            CodeSnippetCompileUnit sequenceCcu = new CodeSnippetCompileUnit(testSequenceSourceCode);
            sequenceCcu.ReferencedAssemblies.AddRange(referencedAssemblies.ToArray());

            CodeGeneratorOptions options = new CodeGeneratorOptions { BracingStyle = "C"};

            //TODO: set StreamWriter line endings to Environment.NewLine

            string sequenceCodePath = Path.Combine(_tempPath, "sequence." + provider.FileExtension);
            using (StreamWriter w = new StreamWriter(sequenceCodePath, false))
                provider.GenerateCodeFromCompileUnit(sequenceCcu, w, options);

            CompilerResults compilerResults = provider.CompileAssemblyFromFile(cp, sequenceCodePath);

            if (compilerResults.Errors.HasErrors)
                throw new CompilerException(compilerResults.Errors);

            Trace.WriteLine("Compiled successfully.");

            return compilerResults.PathToAssembly;
        }

        /// <summary>
        /// Generates a class library (*.dll) assembly from the specified sequence of <see cref="CodeCompileUnit"/>.
        /// </summary>
        /// <param name="codeCompileUnits">A sequence of <see cref="CodeCompileUnit"/> containing the source code and assembly references required for compilation.</param>
        /// <returns>An absolute path to the dynamically generated assembly.</returns>
        public string Compile(IEnumerable<CodeCompileUnit> codeCompileUnits)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider(_language.ToString());

            foreach (string file in Directory.EnumerateFiles(_tempPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                    Trace.WriteLine("Couldn't delete " + file);
                    
                }
            }

            string dllName = Path.ChangeExtension(Path.GetRandomFileName(), "dll");
            string dllPath = Path.Combine(_tempPath, dllName);

            CompilerParameters cp = new CompilerParameters
            {
                IncludeDebugInformation = true,
                OutputAssembly = dllPath
            };
            
#if DEBUG
            cp.CompilerOptions += " /d:DEBUG";
#endif
#if TRACE
            cp.CompilerOptions += " /d:TRACE";
#endif

            foreach (CodeCompileUnit codeCompileUnit in codeCompileUnits)
            {
                cp.ReferencedAssemblies.AddRange(codeCompileUnit.ReferencedAssemblies.Cast<string>().ToArray());

                string fileName = Path.ChangeExtension(Path.GetRandomFileName(), provider.FileExtension);
                string fullPath = Path.Combine(_tempPath, fileName);
                using (StreamWriter w = new StreamWriter(fullPath, false))
                    provider.GenerateCodeFromCompileUnit(codeCompileUnit, w, new CodeGeneratorOptions { BracingStyle = "C" });
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_assemblyDirectory);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            CompilerResults compilerResults = provider.CompileAssemblyFromFile(cp, Directory.GetFiles(_tempPath, "*." + provider.FileExtension));
            Directory.SetCurrentDirectory(currentDirectory);

            if (compilerResults.Errors.HasErrors)
                throw new CompilerException(compilerResults.Errors);

            Trace.WriteLine("Compiled successfully.");

            return compilerResults.PathToAssembly;
        }
    }
}
