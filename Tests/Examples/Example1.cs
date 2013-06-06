using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SilAPI;

namespace Examples
{
    public static class Example1
    {
        public static void DisassembleSomthing()
        {
            //  Disassemble the assembly.
            DisassembledAssembly disassembledAssembly = Disassembler.DisassembleAssembly(@"SomeAssembly.dll");

            //  Write out the assembly to a file.
            using(var stream = new FileStream(@"SomeAssembly.il", FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(disassembledAssembly.RawIL);
            }
        }
    }
}
