using System.Collections.Generic;
using System.Text;

namespace SilAPI
{
    public class DisassembledClass : DisassembledIlClass
    {
        public override string ToString()
        {
            return "Disassembled Class: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            base.InitialiseFromIL();
        }
    }
}
