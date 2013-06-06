using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public class DisassembledEnumeration : DisassembledIlClass
    {
        public override string ToString()
        {
            return "Disassembled Enumeration: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            base.InitialiseFromIL();
        }
    }
}
