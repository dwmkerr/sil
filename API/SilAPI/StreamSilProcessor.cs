using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    /// <summary>
    /// The Sil Processor is the main object that deals with Sil data.
    /// </summary>
    public class StreamSilProcessor : ISilProcessor
    {
        /// <summary>
        /// Generates the IL.
        /// </summary>
        private void GenerateModuleIL()
        {
           using (var reader = new StreamReader(SourceILStream))
           {
                //  Store the output in the module IL.
                moduleIL = reader.ReadToEnd();
           }
        }

        /// <summary>
        /// Parses the file IL.
        /// </summary>
        private void ParseFileIL()
        {
            //  Create a builder for output.
            var builder = new StringBuilder();

            //  Use a string reader to read the module IL.
            using (var reader = new StringReader(moduleIL))
            {
                string line;
                bool inFile = false;
                
                //  Read line by line.
                while ((line = reader.ReadLine()) != null)
                {
                    string filePath;
                    
                    //  Have we got a file part?
                    if (GetSourceFile(line, out filePath) == false)
                    {
                        //  We're not a file part, append the line only if we're in the file.
                        if (inFile)
                            builder.AppendLine(line);
                    }
                    else
                    {
                        //  We're a file part - but are we the right one?
                        inFile = string.CompareOrdinal(Path.GetFullPath(SourceFilePath), Path.GetFullPath(filePath)) == 0;
                    }
                }
            }

            //  Store the file IL.
            fileIL = builder.ToString();
        }

        /// <summary>
        /// Parses the selection IL.
        /// </summary>
        private void ParseSelectionIL()
        {
            //  Create a builder for output.
            var builder = new StringBuilder();

            //  Create a reader on the file IL.
            using (var reader = new StringReader(fileIL))
            {
                string line;
                bool inLines = false;
                while ((line = reader.ReadLine()) != null)
                {
                    //  Get the line parts.
                    int linePart1, linePart2;

                    //  If we have a line like:
                    //  .line 16707566,16707566 : 0,0 ''
                    //  then we must remove every subsequent comment line, its commenting a chunk of code, not 
                    //  line by line code.
                    BurnCommentBlocks(ref line, reader);

                    //  We are in lines if we're between the start and end.
                    if (GetLineHint(line, out linePart1, out linePart2))
                    {
                        inLines = linePart1 >= SourceFileFirstLine;
                        if (inLines && linePart2 > SourceFileLastLine)
                            inLines = false;

                        //  We're on a line hint line, so skip this now, we don't want to show them.
                        continue;
                    }

                    //  If we're not in the lines, move on.
                    if (inLines == false)
                        continue;

                    string comment;
                    if (GetComment(line, out comment, out linePart1))
                    {
                        //  Skip empty comments.
                        if (string.IsNullOrEmpty(comment))
                            continue;

                        //  Re-write comments.
                        line = "\r\n//  Line " + linePart1 + ": " + comment + "\r\n";
                    }

                    //  Add the line to the builder.
                    builder.AppendLine(line);

                }
            }

            //  Store the result.
            selectionIL = builder.ToString();
        }

        private static void BurnCommentBlocks(ref string line, StringReader reader)
        {
            int linePart1, linePart2;

            //  We are in lines if we're between the start and end.
            if (GetLineHint(line, out linePart1, out linePart2))
            {
                //  Have we got the block magic number>
                if (linePart1 == 16707566 && linePart2 == 16707566)
                {
                    string check;
                    while ((check = reader.ReadLine()) != null)
                    {
                        //  Are we a comment?
                        if (check.StartsWith("//") == false)
                            break;
                    }

                    //  Update the current line.
                    line = check;
                }
            }
        }

        private static bool GetSourceFile(string line, out string sourceFilePath)
        {
            sourceFilePath = null;

            //  Source file indicators are as below:
            //  .line 22,22 : 9,10 'C:\\Users\\Dave Kerr\\documents\\visual studio 2010\\Projects\\Sil\\Sil\\ProjectSilProcessor.cs'
            
            //  If we don't have a line indicator, two apostrophes and one colon we're not a line indicator.
            if (line.Contains(".line") == false || line.Count(c => c == '\'') != 2)
                return false;

            //  Get the delimiters.
            var start = line.IndexOf('\'') + 1;
            var end = line.IndexOf('\'', start);

            //  Get the file part.
            var filePart = line.Substring(start, end - start);

            //  If we have no part, bail.
            if (string.IsNullOrEmpty(filePart))
                return false;

            //  We've got a file indicator.
            sourceFilePath = filePart;

            return true;
        }

        private static bool GetComment(string line, out string comment, out int linePart1)
        {
            comment = string.Empty;
            linePart1 = -1;

            //  Comment lines start with //.
            if (line.StartsWith("//") == false)
                return false;

            //  Then have a colon.
            var colonPos = line.IndexOf(':');
            if (colonPos == -1)
                return false;

            //  Between is a number.
            if (Int32.TryParse(line.Substring(2, colonPos - 2), out linePart1) == false)
                return false;

            //  After the colon is the comment.
            comment = line.Substring(colonPos + 1).Trim();

            //  It's a comment.
            return true;
        }

        private static bool GetLineHint(string line, out int firstPart, out int secondPart)
        {
            const string lineToken = ".line ";
            const string endLineToken = " :";

            firstPart = -1;
            secondPart = -1;

            if (line.Contains(lineToken) == false)
                return false;

            //  Sub out the line part.
            int start = line.IndexOf(lineToken, StringComparison.Ordinal) + lineToken.Length;
            int end = line.IndexOf(endLineToken, start, StringComparison.Ordinal);
            if (end == -1)
                return false;

            string lineParts = line.Substring(start, end - start).Trim();

            //  Split on the comma.
            var linePartsArray = lineParts.Split(',').Select(lp => lp.Trim()).ToList();

            //  Convert to ints.
            if (linePartsArray.Count > 0)
                Int32.TryParse(linePartsArray[0], out firstPart);
            if (linePartsArray.Count > 1)
                Int32.TryParse(linePartsArray[1], out secondPart);

            return true;
        }

        /// <summary>
        /// Processes the IL.
        /// </summary>
        /// <returns></returns>
        public virtual bool ProcessIL()
        {
            try
            {
                //  Generate the module IL.
                GenerateModuleIL();

                //  Parse the File IL.
                ParseFileIL();

                //  Parse the selection IL.
                ParseSelectionIL();

                //  We're done.
                return true;
            }
            catch 
            {
                //  Processing the IL failed.
                return false;
            }
        }


        /// <summary>
        /// Gets the selection IL.
        /// </summary>
        /// <returns>
        /// The IL for the selection.
        /// </returns>
        public string GetSelectionIL()
        {
            return selectionIL;
        }

        /// <summary>
        /// Gets the file IL.
        /// </summary>
        /// <returns>
        /// The IL for the file the selection is in.
        /// </returns>
        public string GetFileIL()
        {
            return fileIL;
        }

        /// <summary>
        /// Gets the module IL.
        /// </summary>
        /// <returns>
        /// The IL for the module the selection is in.
        /// </returns>
        public string GetModuleIL()
        {
            return moduleIL;
        }

        /// <summary>
        /// The IL for the module.
        /// </summary>
        protected string moduleIL;

        /// <summary>
        /// The IL for the file.
        /// </summary>
        protected string fileIL;

        /// <summary>
        /// The IL for the selection.
        /// </summary>
        protected string selectionIL;

        /// <summary>
        /// Gets or sets the source IL stream.
        /// </summary>
        /// <value>
        /// The source IL stream.
        /// </value>
        public Stream SourceILStream { get; set; }
        
        /// <summary>
        /// Gets or sets the source file path.
        /// </summary>
        /// <value>
        /// The source file path.
        /// </value>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Gets or sets the source file first line.
        /// </summary>
        /// <value>
        /// The source file first line.
        /// </value>
        public int SourceFileFirstLine { get; set; }

        /// <summary>
        /// Gets or sets the source file last line.
        /// </summary>
        /// <value>
        /// The source file last line.
        /// </value>
        public int SourceFileLastLine { get; set; }
    }
}