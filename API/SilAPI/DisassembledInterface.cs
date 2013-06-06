using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public class DisassembledInterface : DisassembledIlClass
    {
        public override string ToString()
        {
            return "Disassembled Interface: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            base.InitialiseFromIL();
        }
    }
}
