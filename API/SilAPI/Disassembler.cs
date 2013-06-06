using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SilAPI
{
    public class Disassembler : IDisposable
    {
        private Disassembler()
        {
            //  Create the lazy map of file paths to their contents.
            lazyMapFilePathsToContents = new Lazy<Dictionary<string, string>>(CreateMapOfFilePathsToContents);
            lazyMapClassNamesToContents = new Lazy<Dictionary<string, string>>(() => CreateMapOfClassNamesToContents(ILParseHelper.IsLineClassDeclaration));
            lazyMapStructNamesToContents = new Lazy<Dictionary<string, string>>(() => CreateMapOfClassNamesToContents(ILParseHelper.IsLineStructDeclaration));
            lazyMapInterfaceNamesToContents = new Lazy<Dictionary<string, string>>(() => CreateMapOfClassNamesToContents(ILParseHelper.IsLineInterfaceDeclaration));
        }

        /// <summary>
        /// Creates the map of file paths to contents.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> CreateMapOfFilePathsToContents()
        {
            //  Create a dictionary of file paths to their contents.
            var pathsToContents = new Dictionary<string, string>();

            //  Now start parsing the raw IL, line by line.
            using (var reader = new StringReader(rawIl))
            {
                //  Read each line
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    
                }
            }

            //  Return the dictionary.
            return pathsToContents;
        }

        private delegate bool IdentifyClassType(string declarationLine);

        private Dictionary<string, string> CreateMapOfClassNamesToContents(IdentifyClassType identifyClassType)
        {
            //  Create a dictionary of file paths to their contents.
            var pathsToContents = new Dictionary<string, StringBuilder>();

            //  Now start parsing the raw IL, line by line.
            using (var reader = new StringReader(rawIl))
            {
                //  Read each line
                string line;
                string currentClassName = null;
                while ((line = reader.ReadLine()) != null)
                {
                    //  Is it a class start token? If so, get the class name.
                    string templateSpecification;
                    if (identifyClassType(line))
                        ILParseHelper.GetClassNameFromClassDeclarationLine(line, out currentClassName, out templateSpecification);

                    //  If don't have a current class, skip.
                    if (currentClassName == null)
                        continue;
                    
                    //  Add the line to the class.
                    if(!pathsToContents.ContainsKey(currentClassName))
                        pathsToContents[currentClassName] = new StringBuilder();
                    pathsToContents[currentClassName].AppendLine(line);

                    //  Is it a class end token? If so, clear the current class identifier.
                    if (ILParseHelper.IsLineClassEndDeclaration(line, currentClassName))
                        currentClassName = null;
                }
            }

            //  Return the dictionary.
            return pathsToContents.ToDictionary(d => d.Key, d => d.Value.ToString());
        }
        
        public static DisassembledAssembly DisassembleAssembly(string assemblyPath)
        {
            //  Create a disassembled assembly.
            var disassembledAssembly = new DisassembledAssembly();

            //  Set the key properties.
            disassembledAssembly.AssemblyPath = assemblyPath;

            //  Get the assembly name.
            try
            {
                var assembly = Assembly.LoadFile(assemblyPath);
                disassembledAssembly.ShortName = assembly.GetName().Name;
                disassembledAssembly.FullName = assembly.GetName().FullName;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(string.Format("Failed to load the assembly '{0}', it may not be a valid assembly file.", assemblyPath), exception);
            }

            //  Create a temporary file path.
            var tempFilePath = Path.GetTempFileName();

            //  Create an ILDASM object.
            var ildasm = new ILDASM();
            ildasm.IncludeLineNumbers = true;
            ildasm.IncludeOriginalSourceCode = true;
            ildasm.InputFilePath = assemblyPath;
            ildasm.OutputFilePath = tempFilePath;

            //  Disasseble the code.
            try
            {
                ildasm.Run();
            }
            catch (Exception exception)
            {
                //  Add detail and rethrow the exception.
                throw new InvalidOperationException(string.Format("Failed to disassemble '{0}'.", assemblyPath), exception);
            }

            //  Read the file.
            using (var stream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                disassembledAssembly.RawIL = reader.ReadToEnd();
            }
            
            //  Clean up the temporary file.
            File.Delete(tempFilePath);

            //  We're done.
            return disassembledAssembly;
        }
        
        /// <summary>
        /// Initialises disassembler the specified assembly path.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        private void Initialise(string assemblyPath)
        {
            //  Store the assembly path.
            this.assemblyPath = assemblyPath;

            //  Create a temporary file path.
            var tempFilePath = Path.GetTempFileName();

            //  Create an ILDASM object.
            var ildasm = new ILDASM();
            ildasm.IncludeLineNumbers = true;
            ildasm.IncludeOriginalSourceCode = true;
            ildasm.InputFilePath = assemblyPath;
            ildasm.OutputFilePath = tempFilePath;

            //  Disasseble the code.
            try
            {
                ildasm.Run();
            }
            catch (Exception exception)
            {
                //  Add detail and rethrow the exception.
                throw new InvalidOperationException(string.Format("Failed to disassemble '{0}'.", assemblyPath), exception);
            }

            //  Read the file.
            using(var stream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                rawIl = reader.ReadToEnd();
            }

            //  We're done.
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// Gets the raw IL.
        /// </summary>
        /// <returns>The complete raw IL.</returns>
        public string GetRawIL()
        {
            return rawIl;
        }

        /// <summary>
        /// Gets the file names of source code files in the assembly.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFileNames()
        {
            return lazyMapFilePathsToContents.Value.Keys;
        }

        public IEnumerable<string> GetClassNames()
        {
            return lazyMapClassNamesToContents.Value.Keys;
        }

        public IEnumerable<string> GetStructNames()
        {
            return lazyMapStructNamesToContents.Value.Keys;
        }

        public IEnumerable<string> GetInterfaceNames()
        {
            return lazyMapInterfaceNamesToContents.Value.Keys;
        }

        private string assemblyPath;
        private string rawIl;

        /// <summary>
        /// The lazy map of file paths to thier contents.
        /// </summary>
        private Lazy<Dictionary<string, string>> lazyMapFilePathsToContents;

        /// <summary>
        /// The lazy map of class names to their contents.
        /// </summary>
        private Lazy<Dictionary<string, string>> lazyMapClassNamesToContents;
        private Lazy<Dictionary<string, string>> lazyMapStructNamesToContents;
        private Lazy<Dictionary<string, string>> lazyMapInterfaceNamesToContents;
    }
}
