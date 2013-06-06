using System;
using System.IO;
using System.Linq;
using SilAPI;

namespace Examples
{
    public static class Example2
    {
        public static void EnumerateClasses()
        {
            //  Disassemble the assembly.
            DisassembledAssembly disassembledAssembly = Disassembler.DisassembleAssembly(@"SomeAssembly.dll");

            //  Go through each class.
            foreach (var disassembledClass in disassembledAssembly.Classes)
            {
                //  Write out some details on the class.
                Console.WriteLine("Found class: " + disassembledClass.ShortName);
                Console.WriteLine("Fields: " + disassembledClass.Fields.Count());
                Console.WriteLine("Methods: " + disassembledClass.Methods.Count());
            }
        }
    }
}
