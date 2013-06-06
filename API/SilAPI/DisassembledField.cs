using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SilAPI
{
    public class DisassembledField : DisassembledEntity
    {
        public override string ToString()
        {
            return "Disassembled IL Field: " + ShortName ?? "Unknown";
        }

        public override void InitialiseFromIL()
        {
            //  We should have one line of IL for the field.
            string fieldLine;
            using (var reader = new StringReader(RawIL))
            {
                fieldLine = reader.ReadLine();
            }

            //  Clean the line.
            var cleanLine = fieldLine.Trim();

            //  Do we have a literal?
            if (cleanLine.Contains('='))
            {
                var equalsParts = cleanLine.Split('=');
                cleanLine = equalsParts[0].Trim();
                LiteralValue = equalsParts[1].Trim();
            }

            //  We should be in the format
            //  .field modifier modifier modifier type name.
            var parts = cleanLine.Split(' ').ToList();

            //  Remove .field.
            if (parts.Count() > 1)
                parts.RemoveAt(0);

            //  Read name.
            FullName = parts.Last();
            ShortName = FullName;
            parts.RemoveAt(parts.Count - 1);

            //  Read type.
            FieldType = parts.Last();
            parts.RemoveAt(parts.Count - 1);

            modifiers.Clear();
            modifiers.AddRange(parts);
        }

        private readonly List<string> modifiers = new List<string>(); 

        public string FieldType { get; private set; }

        public string LiteralValue { get; private set; }

        public IEnumerable<string> Modifiers { get { return modifiers; } } 
    }
}
