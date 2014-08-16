//-----------------------------------------------------------------------
// <copyright file="DynamicEval.cs" company="Nicholas Armstrong">
//     Created Sept. 2009 by Nicholas Armstrong.  Available online at http://nicholasarmstrong.com
// </copyright>
// <summary>
//     Dynamically compiles C# code into an assembly and executes it.
// </summary>
//-----------------------------------------------------------------------

namespace NicholasArmstrong.Samples.ECE150.Loops
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.CSharp;

    #region Delegates
    /// <summary>
    /// Delegate for the 'Highlight' function.
    /// </summary>
    /// <param name="position">The position of the box to highlight.</param>
    /// <param name="color">The colour to highlight the box.</param>
    public delegate void HighlightDelegate(int position, string color);

    /// <summary>
    /// Delegate for the 'HighlightBox' function.
    /// </summary>
    /// <param name="box">The box to highlight.</param>
    /// <param name="color">The colour to highlight the box.</param>
    public delegate void HighlightBoxDelegate(Box box, string color);

    /// <summary>
    /// Delegate for the 'Wait' function.
    /// </summary>
    /// <param name="millisecondsTimeout">The duration to wait, in milliseconds.</param>
    public delegate void WaitDelegate(int millisecondsTimeout); 
    #endregion

    /// <summary>
    /// Dynamically compiles C# code into an assembly and executes it.
    /// </summary>
    public class DynamicEval
    {
        #region Fields
        /// <summary>
        /// Regular expression to change the 1-argument Highlight function into the two-argument version.
        /// </summary>
        private static readonly Regex highlightOverloadRegex = new Regex("Highlight[(]([^,(]+)[)]");

        /// <summary>
        /// Regular expression to change the 1-argument HighlightBox function into the two-argument version.
        /// </summary>
        private static readonly Regex highlightBoxOverloadRegex = new Regex("HighlightBox[(]([^,(]+)[)]");

        /// <summary>
        /// The 'Highlight()' function.
        /// </summary>
        private HighlightDelegate highlightDelegate;

        /// <summary>
        /// The 'HighlightBox()' function.
        /// </summary>
        private HighlightBoxDelegate highlightBoxDelegate;

        /// <summary>
        /// The 'Wait()' function.
        /// </summary>
        private WaitDelegate waitDelegate;

        /// <summary>
        /// The last compiled assembly.
        /// </summary>
        private Assembly compiledAssembly;

        /// <summary>
        /// Parameters needed for compilation.
        /// </summary>
        private CompilerParameters cp;

        /// <summary>
        /// The C# compiler service.
        /// </summary>
        private CSharpCodeProvider csc;

        /// <summary>
        /// The starting portions of the program that actually gets compiled.
        /// </summary>
        private string programStartText;

        /// <summary>
        /// The ending portions of the program that actually gets compiled.
        /// </summary>
        private string programEndText; 
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the DynamicEval class.
        /// </summary>
        /// <param name="highlightDelegate">An implementation of the 'Highlight' function.</param>
        /// <param name="highlightBoxDelegate">An implementation of the 'HighlightBox' function.</param>
        /// <param name="waitDelegate">An implementation of the 'Wait' function.</param>
        public DynamicEval(HighlightDelegate highlightDelegate, HighlightBoxDelegate highlightBoxDelegate, WaitDelegate waitDelegate)
        {
            this.highlightDelegate = highlightDelegate;
            this.highlightBoxDelegate = highlightBoxDelegate;
            this.waitDelegate = waitDelegate;

            this.Initialize();
        } 
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the dynamic evaluation class.
        /// </summary>
        public void Initialize()
        {
            // Initialize Compiler
            this.cp = new CompilerParameters();
            this.cp.GenerateExecutable = false;
            this.cp.GenerateInMemory = true;
            this.cp.ReferencedAssemblies.Add("ECE150.Loops.dll");
            this.cp.ReferencedAssemblies.Add("mscorlib.dll");
            this.cp.ReferencedAssemblies.Add("System.dll");
            this.cp.ReferencedAssemblies.Add(typeof(Queryable).Assembly.Location);               // Reference LINQ libraries in System.Core
            this.cp.ReferencedAssemblies.Add(typeof(Control).Assembly.Location);                 // Reference WPF's PresentationFramework.dll
            this.cp.ReferencedAssemblies.Add(typeof(UIElement).Assembly.Location);               // Reference WPF's PresentationCore.dll
            this.cp.ReferencedAssemblies.Add(typeof(DependencyProperty).Assembly.Location);      // Reference WPF's WindowsBase.dll

            this.csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });    // Allow LINQ queries to be compiled

            // Initialize static program text
            StringBuilder s = new StringBuilder();
            s.AppendLine("using System;");
            s.AppendLine("using System.Collections.Generic;");
            s.AppendLine("using System.Linq;");
            s.AppendLine("namespace NicholasArmstrong.Samples.ECE150.Loops");
            s.AppendLine("{");
            s.AppendLine("public static class CompiledEval");
            s.AppendLine("{");
            s.AppendLine("public static void HighlightBoxes(HighlightDelegate Highlight, HighlightBoxDelegate HighlightBox, WaitDelegate Wait, int totalBoxes, int boxesPerColumn, int boxesPerRow, IEnumerable<Box> boxes)");
            s.AppendLine("{");
            s.AppendLine("int totalColumns = boxesPerRow;");
            s.AppendLine("int totalRows = boxesPerColumn;");

            this.programStartText = s.ToString();
            this.programEndText = "}}}";
        }

        /// <summary>
        /// Compiles a snippet of code into an assembly.
        /// </summary>
        /// <param name="functionText">The function text to compile.</param>
        /// <param name="errorMessages">Any error messages produced during compilation.</param>
        /// <returns>A value indicating whether compilation was successful.</returns>
        public bool CompileSnippet(string functionText, out string errorMessages)
        {
            // Preprocess program
            functionText = highlightOverloadRegex.Replace(functionText, "Highlight($1, String.Empty)");
            functionText = highlightBoxOverloadRegex.Replace(functionText, "HighlightBox($1, String.Empty)");

            // Construct program
            string programText = this.programStartText + functionText + this.programEndText;

            // Compile program
            CompilerResults results = this.csc.CompileAssemblyFromSource(this.cp, programText);

            // Collect error messages
            errorMessages = String.Empty;
            foreach (CompilerError error in results.Errors)
            {
                // Append error messages, make a rough attempt to hide some of the fancy work we're doing in behind the scenes to compile
                errorMessages += error.ErrorText.Replace("HighlightDelegate", "Highlight").Replace("WaitDelegate", "Wait").Replace("Delegate", "Method").Replace("delegate", "method") + "\n";
            }

            if (errorMessages.Length > 0)
            {
                // Trim off the last newline
                errorMessages = errorMessages.Substring(0, errorMessages.Length - 1);
            }

            this.compiledAssembly = results.Errors.HasErrors ? null : results.CompiledAssembly;
            return !results.Errors.HasErrors;
        }

        /// <summary>
        /// Executes the compiled snippet of code with the provided parameters.
        /// </summary>
        /// <param name="totalBoxes">The total number of boxes in the grid.</param>
        /// <param name="boxesPerColumn">The number of boxes in a grid column.</param>
        /// <param name="boxesPerRow">The number of boxes in a grid row.</param>
        /// <param name="boxes">The collection of boxes themselves (for foreach and LINQ support).</param>
        public void ExecuteSnippet(int totalBoxes, int boxesPerColumn, int boxesPerRow, IEnumerable<Box> boxes)
        {
            if (this.compiledAssembly == null)
            {
                return;
            }

            Type target = this.compiledAssembly.GetTypes()[0];
            MethodInfo method = target.GetMethod("HighlightBoxes");
            object[] parameters = new object[] { this.highlightDelegate, this.highlightBoxDelegate, this.waitDelegate, totalBoxes, boxesPerColumn, boxesPerRow, boxes };
            method.Invoke(null, parameters);
        }
        #endregion
    }
}
