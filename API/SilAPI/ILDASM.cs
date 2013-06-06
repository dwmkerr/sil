using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    /// <summary>
    /// The ILDASM class is a logical wrapper around the ILDASM tool.
    /// </summary>
    public sealed class ILDASM
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ILDASM"/> class.
        /// </summary>
        public ILDASM()
        {
            //  Set sensible default options.
            HideProgressBar = true;
            HideWindow = true;
            IncludeLineNumbers = true;
            IncludeOriginalSourceCode = true;
        }

        /// <summary>
        /// Runs the ILDAMS program, synchronously.
        /// </summary>
        public void Run()
        {
            //  Create the arguments and path.
            var commandLineArguments = BuildCommandLineArguments();
            var path = Path.Combine(GetSDKBinPath(), ILDASMExecutable);

            //  Create the process start info.
            var processStartInfo = new ProcessStartInfo(path, commandLineArguments);

            //  If we're hiding the window, set the appropriate flags.
            if (HideWindow)
            {
                processStartInfo.CreateNoWindow = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            //  Start the process.
            var result = Process.Start(processStartInfo);
            
            //  Wait for the process to finish.
            result.WaitForExit();
        }

        /// <summary>
        /// Builds the command line arguments.
        /// </summary>
        /// <returns>
        /// A string that represents the command line arguemnts for ILDASM.
        /// </returns>
        private string BuildCommandLineArguments()
        {
            //  Check for validity.
            if(string.IsNullOrEmpty(InputFilePath))
                throw new InvalidOperationException("No input file path has been specified.");
            if (string.IsNullOrEmpty(OutputFilePath))
                throw new InvalidOperationException("No output file path has been specified.");

            //  A string builder to build the arguments.
            var stringBuilder = new StringBuilder();

            //  Add the input file path.
            stringBuilder.Append("\"" + InputFilePath + "\"" + Argument_Spacer);

            //  Add the output file path.
            stringBuilder.Append(Argument_OutputFilePath + "\"" + OutputFilePath + "\"" + Argument_Spacer);

            //  Handle the hide progress bar argument.
            if (HideProgressBar)
            {
                stringBuilder.Append(Argument_HideProgressBar);
                stringBuilder.Append(Argument_Spacer);
            }

            //  Handle the line numbers argument.
            if (IncludeLineNumbers)
            {
                stringBuilder.Append(Argument_IncludeLineNumbers);
                stringBuilder.Append(Argument_Spacer);
            }

            //  Handle the include source code argument.
            if (IncludeOriginalSourceCode)
            {
                stringBuilder.Append(Argument_IncldueOriginalSourceCode);
                stringBuilder.Append(Argument_Spacer);
            }

            if (UseHtmlOutput)
            {
                stringBuilder.Append(Argument_UseHtmlOutput);
                stringBuilder.Append(Argument_Spacer);
            }


            //  Return the command line arguments.
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the SDK bin path.
        /// </summary>
        /// <returns>The SDK bin path</returns>
        public static string GetSDKBinPath()
        {
            var candidates = new[]
                                 {
                            @"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\",
                @"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\"
                                 };

            //  Expand the candidates.
            var expandedCandidates = candidates.Select(Environment.ExpandEnvironmentVariables);

            //  Go through each candidate until we find a working one.
            return expandedCandidates.FirstOrDefault(Directory.Exists);

        }
        
        /// <summary>
        /// The ILDASM executable.
        /// </summary>
        private const string ILDASMExecutable = @"ildasm";

        /// <summary>
        /// The spacer between arguments.
        /// </summary>
        private const string Argument_Spacer = @" ";

        /// <summary>
        /// The ouput file path argument.
        /// </summary>
        private const string Argument_OutputFilePath = @"/output:";

        /// <summary>
        /// The Hide Progress Bar argument.
        /// </summary>
        private const string Argument_HideProgressBar = @"/nobar";

        /// <summary>
        /// The Include Line Numbers argument.
        /// </summary>
        private const string Argument_IncludeLineNumbers = @"/linenum";

        /// <summary>
        /// The Include Original Source Code argument.
        /// </summary>
        private const string Argument_IncldueOriginalSourceCode = @"/source";

        /// <summary>
        /// The argument to use HTML output.
        /// </summary>
        private const string Argument_UseHtmlOutput = @"/html";

        /// <summary>
        /// Gets or sets the input file path.
        /// </summary>
        /// <value>
        /// The input file path.
        /// </value>
        public string InputFilePath { get; set; }

        /// <summary>
        /// Gets or sets the output file path.
        /// </summary>
        /// <value>
        /// The output file path.
        /// </value>
        public string OutputFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide the ILDASM window.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hide window]; otherwise, <c>false</c>.
        /// </value>
        public bool HideWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide the progress bar.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hide progress bar; otherwise, <c>false</c>.
        /// </value>
        public bool HideProgressBar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include line numbers].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include line numbers]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeLineNumbers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include original source code].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [include original source code]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeOriginalSourceCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use HTML output.
        /// </summary>
        /// <value>
        ///   <c>true</c> if HTML output should be used; otherwise, <c>false</c>.
        /// </value>
        public bool UseHtmlOutput { get; set; }
    }
}
