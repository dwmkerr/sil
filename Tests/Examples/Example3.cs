using System;
using SilAPI;

namespace Examples
{
    public static class Example3
    {
        public static void SearchAssembly()
        {
            //  Disassemble the assembly.
            DisassembledAssembly disassembledAssembly = Disassembler.DisassembleAssembly(@"SomeAssembly.dll");

            //  Create a disassembly target that targets the interface named 'ISomeInterface'.
            DisassemblyTarget target = new DisassemblyTarget(DisassemblyTargetType.Interface, @"SimeAssembly.ISomeInterface");

            //  Search for the interface.
            DisassembledEntity entity = disassembledAssembly.FindDisassembledEntity(target);

            //  If we found it, show the IL.
            if(entity != null)
                Console.WriteLine(entity.RawIL);
        }
    }
}