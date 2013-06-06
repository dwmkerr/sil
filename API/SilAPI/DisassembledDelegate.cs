using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public class DisassembledDelegate : DisassembledIlClass
    {
        public override string ToString()
        {
            return "Disassembled Delegate: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            base.InitialiseFromIL();
        }
    }
}
