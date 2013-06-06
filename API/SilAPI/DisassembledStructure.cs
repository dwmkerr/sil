using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public class DisassembledStructure : DisassembledIlClass
    {
        public override string ToString()
        {
            return "Disassembled Structure: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            base.InitialiseFromIL();
        }
    }
}
