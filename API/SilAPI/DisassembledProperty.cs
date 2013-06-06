using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public class DisassembledProperty : DisassembledEntity
    {
        public override string ToString()
        {
            return "Disassembled Property: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            string line;
            using (var reader = new StringReader(RawIL))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    var methodName = ILParseHelper.GetPropertyNameFromDeclarationLine(line);
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
