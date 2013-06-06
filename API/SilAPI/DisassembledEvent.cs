using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public class DisassembledEvent : DisassembledEntity
    {
        public override string ToString()
        {
            return "Disassembled Event: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            string line;
            using (var reader = new StringReader(RawIL))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var methodName = ILParseHelper.GetEventNameFromDeclarationLine(line);
                    if (methodName != null)
                    {
                        ShortName = methodName;
                        FullName = methodName;
                        break;
                    }
                }
            }
        }
    }
}
