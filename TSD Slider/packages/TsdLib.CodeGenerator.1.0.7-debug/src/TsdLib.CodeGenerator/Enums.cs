using System;

namespace TsdLib.CodeGenerator
{
    /// <summary>
    /// Describes the types of input that are supported.
    /// </summary>
    public enum CodeType
    {
        /// <summary>
        /// An *.xml instrument definition file.
        /// </summary>
        Instruments,
        /// <summary>
        /// A *.cs or *.vb test sequence class file.
        /// </summary>
        TestSequence
    }

    /// <summary>
    /// Describes the types of output that can be generated.
    /// </summary>
    [Flags]
    public enum OutputFormat
    {
        /// <summary>
        /// Source code.
        /// </summary>
        Source = 1,
        /// <summary>
        /// Compiled assembly.
        /// </summary>
        Assembly = 2,
        /// <summary>
        /// Source code and a compiled assembly.
        /// </summary>
        Both = Source | Assembly
    }

    /// <summary>
    /// Describes the programming languages that are supported for code input or output.
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// C# language.
        /// </summary>
        CSharp,
        /// <summary>
        /// Visual Basic language.
        /// </summary>
        VisualBasic
    }
}